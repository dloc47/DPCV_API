using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPCV_API.BAL.Services.Website.Activities;
using DPCV_API.Models.ActivityModel;

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

        // ✅ Get All Activities
        [HttpGet]
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
                return BadRequest(ModelState);
            }

            var result = await _activityService.CreateActivityAsync(activity, User);
            if (!result)
            {
                return Forbid();
            }

            return CreatedAtAction(nameof(GetActivityById), new { id = activity.ActivityId }, activity);
        }

        // ✅ Update Activity with Role Validation
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityDTO activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _activityService.UpdateActivityAsync(id, activity, User);
            if (!result)
            {
                return Forbid();
            }

            return NoContent();
        }

        // ✅ Delete Activity with Role Validation
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id, User);
            if (!result)
            {
                return Forbid();
            }

            return NoContent();
        }
    }
}
