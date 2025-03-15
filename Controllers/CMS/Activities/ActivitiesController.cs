using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPCV_API.Models.ActivityModel;
using DPCV_API.BAL.Services.Activities;

namespace DPCV_API.Controllers.Website
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(IActivityService activityService, ILogger<ActivitiesController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        // ✅ Get Paginated Activities
        [HttpGet("paginated-activities")]
        public async Task<IActionResult> GetPaginatedActivities([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                    return BadRequest(new { message = "Page number and page size must be greater than zero." });

                var activities = await _activityService.GetPaginatedActivitiesAsync(pageNumber, pageSize);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated activities.");
                return StatusCode(500, new { message = "An error occurred while fetching activities." });
            }
        }

        // ✅ Get All Activities
        [HttpGet("all-activities")]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        // ✅ Get Activity by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                return NotFound(new { message = "Activity not found" });
            }
            return Ok(activity);
        }

        // ✅ Create Activity with Role Validation
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityDTO activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _activityService.CreateActivityAsync(activity, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to create this activity." });
            }

            return Ok(new { message = "Activity created successfully." });
        }



        // ✅ Update Activity with Role Validation
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityDTO activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _activityService.UpdateActivityAsync(id, activity, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to update this activity or no changes were made." });
            }

            return Ok(new { message = "Activity updated successfully." });
        }


        // ✅ Delete Activity with Role Validation
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to delete this activity or it does not exist." });
            }

            return Ok(new { message = "Activity deleted successfully." });
        }

        // ✅ Archive Activity (Set is_active = 0)
        [HttpPut("{id}/archive")]
        [Authorize]
        public async Task<IActionResult> ArchiveActivity(int id)
        {
            var (success, message) = await _activityService.ArchiveActivityAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        // ✅ Unarchive Activity (Set is_active = 1)
        [HttpPut("{id}/unarchive")]
        [Authorize]
        public async Task<IActionResult> UnarchiveActivity(int id)
        {
            var (success, message) = await _activityService.UnarchiveActivityAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }



    }
}
