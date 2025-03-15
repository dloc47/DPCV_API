using DPCV_API.Configuration.DbContext;
using DPCV_API.Helpers;
using DPCV_API.Models.ActivityModel;
using DPCV_API.Models.CommonModel;
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

        // ✅ Get Paginated User
        public async Task<PaginatedResponse<UserResponseDTO>> GetPaginatedUsersAsync(int pageNumber, int pageSize)
        {
            string procedureName = "GetPaginatedUsers";

            var parameters = new Dictionary<string, object>
    {
        { "PageNumber", pageNumber },
        { "PageSize", pageSize }
    };

            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure, parameters);
            List<UserResponseDTO> users = new();

            foreach (DataRow row in result.Rows)
            {
                users.Add(new UserResponseDTO
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Name = row["name"].ToString() ?? string.Empty,
                    Email = row["email"].ToString() ?? string.Empty,
                    RoleId = Convert.ToInt32(row["role_id"]),
                    RoleName = row["role_name"]?.ToString() ?? string.Empty,
                    CommitteeId = row["committee_id"] != DBNull.Value ? Convert.ToInt32(row["committee_id"]) : (int?)null,
                    CommitteeName = row["committee_name"]?.ToString(),
                    DistrictId = row["district_id"] != DBNull.Value ? Convert.ToInt32(row["district_id"]) : (int?)null,
                    DistrictName = row["district_name"] != DBNull.Value ? row["district_name"].ToString() : null,
                    Region = row["region"] != DBNull.Value ? row["region"].ToString() : null,
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"])
                });
            }

            return new PaginatedResponse<UserResponseDTO>
            {
                Data = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = await GetTotalUserCountAsync()
            };
        }

        // ✅ Get total user count
        private async Task<int> GetTotalUserCountAsync()
        {
            string query = "SELECT COUNT(*) FROM users";

            object? result = await _dataManager.ExecuteScalarAsync<object>(query, CommandType.Text);

            return result != null ? Convert.ToInt32(result) : 0;
        }


        // ✅ Get All Users (Accessible to everyone)
        public async Task<List<UserResponseDTO>?> GetAllUsersAsync()
        {
            string procedureName = "GetAllUsers";

            // Execute stored procedure and get result set as DataTable
            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure);
            List<UserResponseDTO> users = new();

            // Iterate through each row and map to UserResponseDTO
            foreach (DataRow row in result.Rows)
            {
                users.Add(new UserResponseDTO
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Name = row["name"].ToString() ?? string.Empty,
                    Email = row["email"].ToString() ?? string.Empty,
                    //Password = row["password"].ToString() ?? string.Empty, // Uncomment this line if we need password for GetUsers, uncomment also in StoredProcedure and UserDTO model class
                    RoleId = Convert.ToInt32(row["role_id"]),
                    RoleName = row["role_name"]?.ToString() ?? string.Empty,
                    CommitteeId = row["committee_id"] != DBNull.Value ? Convert.ToInt32(row["committee_id"]) : (int?)null,
                    CommitteeName = row["committee_name"]?.ToString(),
                    DistrictId = row["district_id"] != DBNull.Value ? Convert.ToInt32(row["district_id"]) : (int?)null, // ✅ Fix: Use null instead of 0
                    DistrictName = row["district_name"] != DBNull.Value ? row["district_name"].ToString() : null, // ✅ Fix: Use null instead of ""
                    Region = row["region"] != DBNull.Value ? row["region"].ToString() : null, // ✅ Fix: Use null instead of ""
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"])
                });
            }

            return users;
        }


        // ✅ Get User by ID (Accessible to everyone)
        public async Task<UserResponseDTO?> GetUserByIdAsync(int userId)
        {
            string procedureName = "GetUserById";

            // Clear previous parameters and add the user ID as a parameter
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_UserId", userId);

            // Execute stored procedure and get result set as DataTable
            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure);

            // If no record is found, return null
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];

            // Map the data row to UserResponseDTO and return
            return new UserResponseDTO
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Name = row["name"].ToString() ?? string.Empty,
                Email = row["email"].ToString() ?? string.Empty,
                //Password = row["password"].ToString() ?? string.Empty, // Uncomment this line if we need password for GetUsers, uncomment also in StoredProcedure and UserDTO model class
                RoleId = Convert.ToInt32(row["role_id"]),
                RoleName = row["role_name"]?.ToString() ?? string.Empty,
                CommitteeId = row["committee_id"] != DBNull.Value ? Convert.ToInt32(row["committee_id"]) : (int?)null,
                CommitteeName = row["committee_name"]?.ToString(),
                DistrictId = row["district_id"] != DBNull.Value ? Convert.ToInt32(row["district_id"]) : (int?)null, // ✅ Fix: Use null instead of 0
                DistrictName = row["district_name"] != DBNull.Value ? row["district_name"].ToString() : null, // ✅ Fix: Use null instead of ""
                Region = row["region"] != DBNull.Value ? row["region"].ToString() : null, // ✅ Fix: Use null instead of ""
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }


        // ✅ Create User (Only Admin can create users)
        public async Task<bool> CreateUserAsync(UserDTO userDto, ClaimsPrincipal user)
        {
            _logger.LogInformation("User Claims: {Claims}", string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

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
            _dataManager.AddParameter("@p_CommitteeId", userDto.CommitteeId ?? (object)DBNull.Value); // ✅ Handle null committee ID
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
            _dataManager.AddParameter("@p_CommitteeId", userDto.CommitteeId ?? (object)DBNull.Value); // ✅ Handle null committee ID
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
