using DPCV_API.Models.CMS.Users;
using DPCV_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.CMS.Users
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ Get user by ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // ✅ Create user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            bool isCreated = await _userService.CreateUserAsync(user);
            if (!isCreated) return BadRequest("Failed to create user.");
            return CreatedAtAction(nameof(GetUserById), new { userId = user.UserId }, user);
        }

        // ✅ Update user
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDTO user)
        {
            bool isUpdated = await _userService.UpdateUserAsync(userId, user);
            if (!isUpdated) return BadRequest("Failed to update user.");
            return NoContent();
        }

        // ✅ Delete user
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            bool isDeleted = await _userService.DeleteUserAsync(userId);
            if (!isDeleted) return BadRequest("Failed to delete user.");
            return NoContent();
        }

        // ✅ Activate/Deactivate user
        [HttpPatch("{userId}/status")]
        public async Task<IActionResult> SetUserActiveStatus(int userId, [FromBody] bool isActive)
        {
            bool isUpdated = await _userService.SetUserActiveStatusAsync(userId, isActive);
            if (!isUpdated) return BadRequest("Failed to update user status.");
            return NoContent();
        }
    }
}
