using DPCV_API.BAL.Services.Homestays;
using DPCV_API.Models.HomestayModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website
{
    [Route("api/homestays")]
    [ApiController]
    public class HomestayController : ControllerBase
    {
        private readonly IHomestayService _homestayService;
        private readonly ILogger<HomestayController> _logger;

        public HomestayController(IHomestayService homestayService, ILogger<HomestayController> logger)
        {
            _homestayService = homestayService;
            _logger = logger;
        }

        // ✅ Get Paginated Homestays
        [HttpGet("paginated-homestays")]
        public async Task<IActionResult> GetPaginatedHomestays([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Invalid pagination parameters." });
                }

                var result = await _homestayService.GetPaginatedHomestaysAsync(pageNumber, pageSize);

                return result.Data.Any() ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated homestays.");
                return StatusCode(500, new { message = "An error occurred while fetching homestay data." });
            }
        }

        // ✅ Get All Homestays
        [HttpGet]
        public async Task<IActionResult> GetAllHomestays()
        {
            var result = await _homestayService.GetAllHomestaysAsync();
            return Ok(result);
        }

        // ✅ Get Homestay by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomestayById(int id)
        {
            var result = await _homestayService.GetHomestayByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Homestay not found" });
            }
            return Ok(result);
        }

        // ✅ Create Homestay
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateHomestay([FromBody] HomestayDTO homestayDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _homestayService.CreateHomestayAsync(homestayDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to create this Homestay." });
            }

            return Ok(new { message = "Homestay created successfully." });
        }

        // ✅ Update Homestay
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHomestay(int id, [FromBody] HomestayDTO homestayDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            homestayDto.HomestayId = id;
            var result = await _homestayService.UpdateHomestayAsync(homestayDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to update this Homestay or no changes were made." });
            }

            return Ok(new { message = "Homestay updated successfully." });
        }

        // ✅ Delete Homestay
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHomestay(int id)
        {
            var result = await _homestayService.DeleteHomestayAsync(id, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to delete this Homestay or it does not exist." });
            }

            return Ok(new { message = "Homestay deleted successfully." });
        }

        // ✅ Archive Homestay (Set is_active = 0)
        [Authorize]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveHomestay(int id)
        {
            var (success, message) = await _homestayService.ArchiveHomestayAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        // ✅ Unarchive Homestay (Set is_active = 1)
        [Authorize]
        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnarchiveHomestay(int id)
        {
            var (success, message) = await _homestayService.UnarchiveHomestayAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }
    }
}
