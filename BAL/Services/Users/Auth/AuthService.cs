using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DPCV_API.Models.Users;

namespace DPCV_API.BAL.Services.Users.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DataManager _dataManager;
        private readonly IConfiguration _config;

        // Inject DataManager via constructor
        public AuthService(IConfiguration config, DataManager dataManager)
        {
            _dataManager = dataManager;
            _config = config;
        }

        public async Task<ServiceResult> RegisterUserAsync(RegisterDTO registerDto, ClaimsPrincipal user)
        {
            // Check if the user is authorized (Only Admins can register users)
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim == null || roleClaim.Value != "1") // 1 = Admin
            {
                return new ServiceResult { Errors = new List<string> { "Unauthorized: Only admins can register users." } };
            }

            if (registerDto.RoleId == 2 && registerDto.DistrictId == null) // Committee must have DistrictId
            {
                return new ServiceResult { Errors = new List<string> { "District ID is required for Committee users." } };
            }

            // Check if the email already exists
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_email", registerDto.Email);
            DataTable dt = await _dataManager.ExecuteQueryAsync("GetUserByEmail", CommandType.StoredProcedure);
            if (dt.Rows.Count > 0)
            {
                return new ServiceResult { Errors = new List<string> { "User already exists with this email." } };
            }

            // Register user
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_name", registerDto.Name);
            _dataManager.AddParameter("p_email", registerDto.Email);
            _dataManager.AddParameter("p_password", ComputeSha256Hash(registerDto.Password));
            _dataManager.AddParameter("p_role_id", registerDto.RoleId);
            _dataManager.AddParameter("p_district_id", registerDto.RoleId == 1 ? DBNull.Value : registerDto.DistrictId);

            bool success = await _dataManager.ExecuteNonQueryAsync("RegisterUser", CommandType.StoredProcedure);

            return success
                ? new ServiceResult { Message = "User registered successfully." }
                : new ServiceResult { Errors = new List<string> { "Registration failed due to a database error." } };
        }

        public async Task<ServiceResult> LoginUserAsync(AuthDTO authDto)
        {
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_email", authDto.Email);

            DataTable dt = await _dataManager.ExecuteQueryAsync("GetUserByEmail", CommandType.StoredProcedure);
            var user = dt.ConvertToTargetTypeList<UserDTO>().FirstOrDefault();

            if (user == null)
            {
                return new ServiceResult { Errors = new List<string> { "User does not exist." } };
            }
            else if (!user.IsActive)
            {
                return new ServiceResult { Errors = new List<string> { "Account is inactive. Please contact support." } };
            }
            else if (user.Password != ComputeSha256Hash(authDto.Password))
            {
                return new ServiceResult { Errors = new List<string> { "Incorrect password." } };
            }

            // Ensure only Admin (1) and Committee (2) can log in
            if (user.RoleId != 1 && user.RoleId != 2)
            {
                return new ServiceResult { Errors = new List<string> { "Unauthorized: Only Admins and Committees can log in." } };
            }

            var tokens = GenerateTokens(user);

            return new ServiceResult
            {
                Data = tokens,
                Message = "Login successful."
            };
        }


        public async Task<ServiceResult> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto)
        {
            // Validate the expired token and extract user details
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.RefreshToken);
            if (principal == null)
            {
                return new ServiceResult { Errors = new List<string> { "Invalid refresh token." } };
            }

            int userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if the refresh token exists and is not revoked
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_user_id", userId);
            _dataManager.AddParameter("p_refresh_token", refreshTokenDto.RefreshToken);

            DataTable dt = await _dataManager.ExecuteQueryAsync("ValidateRefreshToken", CommandType.StoredProcedure);
            if (dt.Rows.Count == 0)
            {
                return new ServiceResult { Errors = new List<string> { "Invalid or expired refresh token." } };
            }

            var storedToken = dt.ConvertToTargetTypeList<RefreshTokenDTO>().FirstOrDefault();
            if (storedToken == null || storedToken.IsRevoked)
            {
                return new ServiceResult { Errors = new List<string> { "Refresh token has been revoked." } };
            }

            // Generate new tokens
            var tokens = GenerateTokens(new UserDTO { UserId = userId });

            // Revoke the old token
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_user_id", userId);
            _dataManager.AddParameter("p_refresh_token", refreshTokenDto.RefreshToken);
            await _dataManager.ExecuteNonQueryAsync("RevokeRefreshToken", CommandType.StoredProcedure);

            // Insert the new refresh token
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_user_id", userId);
            _dataManager.AddParameter("p_refresh_token", tokens.RefreshToken);
            _dataManager.AddParameter("p_expires_at", DateTime.UtcNow.AddDays(7));
            bool updateSuccess = await _dataManager.ExecuteNonQueryAsync("InsertRefreshToken", CommandType.StoredProcedure);

            if (!updateSuccess)
            {
                return new ServiceResult { Errors = new List<string> { "Failed to update refresh token." } };
            }

            return new ServiceResult
            {
                Data = tokens,
                Message = "Token refreshed successfully."
            };
        }

        public async Task<ServiceResult> LogoutAsync(int userId)
        {
            _dataManager.ClearParameters();
            _dataManager.AddParameter("p_user_id", userId);
            await _dataManager.ExecuteNonQueryAsync("RevokeAllRefreshTokens", CommandType.StoredProcedure);

            return new ServiceResult { Message = "Logged out successfully." };
        }



        private AuthResponseDTO GenerateTokens(UserDTO user)
        {
            string token = GenerateJwtToken(user);
            string refreshToken = GenerateRefreshToken();

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                RoleId = user.RoleId,
                UserId = user.UserId
            };
        }

        private string GenerateJwtToken(UserDTO user)
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key) || key.Length < 32)
            {
                throw new Exception("JWT Secret Key is missing or too short. It must be at least 32 characters.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
                //new Claim("role", user.RoleId.ToString()) // Use "role" instead of `ClaimTypes.Role`
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])), // ✅ Uses ExpireMinutes
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new Exception("JWT key is missing"));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // We only validate expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal format
                }
                return builder.ToString();
            }
        }
    }
}
