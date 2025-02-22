using Microsoft.AspNetCore.Mvc;
using DPCV_API.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DPCV_API.BAL.Services.CMS.Auth;
using DPCV_API.Models.CMS.Users;

namespace DPCV_API.Controllers.CMS.Users
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var user = HttpContext.User; // Get the authenticated user
            ServiceResult result = await _authService.RegisterUserAsync(registerDto, user);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTO authDto)
        {
            ServiceResult result = await _authService.LoginUserAsync(authDto);

            if (!result.IsSuccessful)
            {
                return Unauthorized(result); // HTTP 401 for invalid login
            }

            return Ok(result);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
        {
            ServiceResult result = await _authService.RefreshTokenAsync(refreshTokenDto);
            if (!result.IsSuccessful)
            {
                return Unauthorized(result); // HTTP 401 if refresh token is invalid
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ServiceResult { Errors = new List<string> { "Invalid user ID." } });
            }

            var result = await _authService.LogoutAsync(int.Parse(userId));
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [Authorize] // ✅ Requires authentication
        [HttpGet("me")]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleId = User.FindFirst(ClaimTypes.Role)?.Value;
            var districtId = User.FindFirst("DistrictId")?.Value;

            return Ok(new
            {
                UserId = userId,
                RoleId = roleId,
                DistrictId = districtId
            });
        }
    }
}
