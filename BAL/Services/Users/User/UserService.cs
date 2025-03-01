using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Users;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DPCV_API.BAL.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<UserService> _logger;

        public UserService(DataManager dataManager, ILogger<UserService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        // ✅ Get All Users (Accessible to everyone)
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            string spName = "GetAllUsers";
            List<UserDTO> users = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    users.Add(MapUser(row));
                }

                _logger.LogInformation("Fetched {Count} users successfully.", users.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users.");
                throw;
            }

            return users;
        }

        // ✅ Get User by ID (Accessible to everyone)
        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            string spName = "GetUserById";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_UserId", userId);

                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return null;
                }

                _logger.LogInformation("Fetched user with ID {UserId}.", userId);
                return MapUser(result.Rows[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID {UserId}.", userId);
                throw;
            }
        }


        // ✅ Create User (Only Admin can create users)
        public async Task<bool> CreateUserAsync(UserDTO userDto, ClaimsPrincipal user)
        {
            if (!IsAdmin(user))
            {
                _logger.LogWarning("Unauthorized attempt to create user.");
                throw new UnauthorizedAccessException("Only admins can create users.");
            }

            string spName = "CreateUser";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_Name", userDto.Name);
            _dataManager.AddParameter("@p_Email", userDto.Email);
            _dataManager.AddParameter("@p_Password", HashPassword(userDto.Password));
            _dataManager.AddParameter("@p_RoleId", userDto.RoleId);
            _dataManager.AddParameter("@p_CommitteeId", userDto.CommitteeId);
            _dataManager.AddParameter("@p_IsActive", userDto.IsActive);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(spName, CommandType.StoredProcedure);
                _logger.LogInformation("User {Email} created successfully.", userDto.Email);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}.", userDto.Email);
                throw;
            }
        }

        // ✅ Update User (Admins can update anyone, Committee users can only update their own profile)
        public async Task<bool> UpdateUserAsync(UserDTO userDto, ClaimsPrincipal user)
        {
            var currentUserId = GetUserId(user);
            var currentUserRole = GetUserRole(user);

            if (currentUserRole == "Committee" && userDto.UserId != currentUserId)
            {
                _logger.LogWarning("Unauthorized attempt to update user ID {UserId}.", userDto.UserId);
                throw new UnauthorizedAccessException("You can only update your own profile.");
            }

            string spName = "UpdateUser";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_UserId", userDto.UserId);
            _dataManager.AddParameter("@p_Name", userDto.Name);
            _dataManager.AddParameter("@p_Email", userDto.Email);
            _dataManager.AddParameter("@p_Password", HashPassword(userDto.Password));
            _dataManager.AddParameter("@p_CommitteeId", userDto.CommitteeId);
            _dataManager.AddParameter("@p_IsActive", userDto.IsActive);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(spName, CommandType.StoredProcedure);
                _logger.LogInformation("User {UserId} updated successfully.", userDto.UserId);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}.", userDto.UserId);
                throw;
            }
        }

        // ✅ Delete User (Only Admin can delete users)
        public async Task<bool> DeleteUserAsync(int userId, ClaimsPrincipal user)
        {
            if (!IsAdmin(user))
            {
                _logger.LogWarning("Unauthorized attempt to delete user ID {UserId}.", userId);
                throw new UnauthorizedAccessException("Only admins can delete users.");
            }

            string spName = "DeleteUser";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_UserId", userId);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(spName, CommandType.StoredProcedure);
                _logger.LogInformation("User {UserId} deleted successfully.", userId);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}.", userId);
                throw;
            }
        }

        // ✅ Archive User (Only Admin can archive)
        public async Task<(bool success, string message)> ArchiveUserAsync(int userId, ClaimsPrincipal user)
        {
            if (!IsAdmin(user))
            {
                _logger.LogWarning("Unauthorized attempt to archive user ID {UserId}.", userId);
                return (false, "Only admins can archive users.");
            }

            string spName = "ArchiveUser";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_UserId", userId);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(spName, CommandType.StoredProcedure);
                if (success)
                    _logger.LogInformation("User {UserId} archived successfully.", userId);
                else
                    _logger.LogWarning("Failed to archive user {UserId}.", userId);

                return success ? (true, "User archived successfully.") : (false, "Failed to archive user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving user {UserId}.", userId);
                throw;
            }
        }

        // ✅ Unarchive User (Only Admin can unarchive)
        public async Task<(bool success, string message)> UnarchiveUserAsync(int userId, ClaimsPrincipal user)
        {
            if (!IsAdmin(user))
            {
                _logger.LogWarning("Unauthorized attempt to unarchive user ID {UserId}.", userId);
                return (false, "Only admins can unarchive users.");
            }

            string spName = "UnarchiveUser";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_UserId", userId);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(spName, CommandType.StoredProcedure);
                if (success)
                    _logger.LogInformation("User {UserId} unarchived successfully.", userId);
                else
                    _logger.LogWarning("Failed to unarchive user {UserId}.", userId);

                return success ? (true, "User unarchived successfully.") : (false, "Failed to unarchive user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unarchiving user {UserId}.", userId);
                throw;
            }
        }

        // ✅ Map DataRow to UserDTO
        private UserDTO MapUser(DataRow row)
        {
            return new UserDTO
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Name = row["name"].ToString()!,
                Email = row["email"].ToString()!,
                Password = row["password"].ToString()!,
                RoleId = Convert.ToInt32(row["role_id"]),
                CommitteeId = row["committee_id"] != DBNull.Value ? Convert.ToInt32(row["committee_id"]) : null,
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        // ✅ Hash Password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // ✅ Helper: Check if user is an Admin
        private bool IsAdmin(ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        // ✅ Helper: Get User ID from Claims
        private int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : 0;
        }

        // ✅ Helper: Get User Role from Claims
        private string GetUserRole(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
        }
    }
}
