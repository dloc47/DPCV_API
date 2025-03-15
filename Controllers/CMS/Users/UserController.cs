using DPCV_API.BAL.Services.Users;
using DPCV_API.Controllers.Website;
using DPCV_API.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.CMS
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ✅ Get Paginated Users (Accessible to everyone)
        [HttpGet("paginated-users")]
        public async Task<IActionResult> GetPaginatedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Invalid pagination parameters." });
                }

                var result = await _userService.GetPaginatedUsersAsync(pageNumber, pageSize);

                return result.Data.Any() ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated users.");
                return StatusCode(500, new { message = "An error occurred while fetching user data." });
            }
        }


        // ✅ Get All Users (Accessible to everyone)
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ Get User by ID (Accessible to everyone)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(user);
        }

        // ✅ Create User (Only Admin can create users)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            try
            {
                var result = await _userService.CreateUserAsync(userDto, User);

                if (!result)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "You are not authorized to create a user." });
                }

                return Ok(new { message = "User created successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // ✅ Update User (Admins can update anyone, Committee users can only update their own profile)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            userDto.UserId = id; // Ensure the ID from URL matches the DTO

            try
            {
                var result = await _userService.UpdateUserAsync(userDto, User);

                if (!result)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "You are not authorized to update this user or no changes were made." });
                }

                return Ok(new { message = "User updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // ✅ Delete User (Only Admin can delete users)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id, User);

                if (!result)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "You are not authorized to delete this user or it does not exist." });
                }

                return Ok(new { message = "User deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        // ✅ Archive User (Set is_active = 0)
        [Authorize]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            var (success, message) = await _userService.ArchiveUserAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        // ✅ Unarchive User (Set is_active = 1)
        [Authorize]
        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnarchiveUser(int id)
        {
            var (success, message) = await _userService.UnarchiveUserAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }
    }
}
