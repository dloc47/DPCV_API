using DPCV_API.Configuration.DbContext;
using DPCV_API.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace DPCV_API.Services
{
    public class UserService : IUserService
    {
        private readonly DataManager _dataManager;

        // Inject DataManager via constructor
        public UserService(DataManager dataManager)
        {
            _dataManager = dataManager;
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

        // ✅ Get All Users
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            string query = "SELECT * FROM Users";
            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            List<UserDTO> users = new();

            foreach (DataRow row in result.Rows)
            {
                users.Add(new UserDTO
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Name = row["name"].ToString()!,
                    Email = row["email"].ToString()!,
                    Password = row["password"].ToString()!,
                    RoleId = Convert.ToInt32(row["role_id"]),
                    DistrictId = row["district_id"] != DBNull.Value ? Convert.ToInt32(row["district_id"]) : null,
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"])
                });
            }
            return users;
        }

        // ✅ Get User by ID
        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            string query = "SELECT * FROM Users WHERE user_id = @UserId";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@UserId", userId);

            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];
            return new UserDTO
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Name = row["name"].ToString()!,
                Email = row["email"].ToString()!,
                Password = row["password"].ToString()!,
                RoleId = Convert.ToInt32(row["role_id"]),
                DistrictId = row["district_id"] != DBNull.Value ? Convert.ToInt32(row["district_id"]) : null,
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        // ✅ Create New User
        public async Task<bool> CreateUserAsync(UserDTO user)
        {
            string query = "INSERT INTO Users (name, email, password, role_id, district_id, is_active) VALUES (@Name, @Email, @Password, @RoleId, @DistrictId, @IsActive)";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@Name", user.Name);
            _dataManager.AddParameter("@Email", user.Email);
            _dataManager.AddParameter("@Password", HashPassword(user.Password));
            _dataManager.AddParameter("@RoleId", user.RoleId);
            _dataManager.AddParameter("@DistrictId", user.DistrictId);
            _dataManager.AddParameter("@IsActive", user.IsActive);

            return await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
        }

        // ✅ Update User
        public async Task<bool> UpdateUserAsync(int userId, UserDTO user)
        {
            string query = "UPDATE Users SET name = @Name, email = @Email, password = @Password, role_id = @RoleId, district_id = @DistrictId, is_active = @IsActive WHERE user_id = @UserId";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@UserId", userId);
            _dataManager.AddParameter("@Name", user.Name);
            _dataManager.AddParameter("@Email", user.Email);
            _dataManager.AddParameter("@Password", HashPassword(user.Password));
            _dataManager.AddParameter("@RoleId", user.RoleId);
            _dataManager.AddParameter("@DistrictId", user.DistrictId);
            _dataManager.AddParameter("@IsActive", user.IsActive);

            return await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
        }

        // ✅ Delete User
        public async Task<bool> DeleteUserAsync(int userId)
        {
            string query = "DELETE FROM Users WHERE user_id = @UserId";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@UserId", userId);
            return await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
        }

        // ✅ Activate/Deactivate User
        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            string query = "UPDATE Users SET is_active = @IsActive WHERE user_id = @UserId";

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@UserId", userId);
            _dataManager.AddParameter("@IsActive", isActive);

            return await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
        }
    }
}
