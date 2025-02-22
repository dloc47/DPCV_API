using DPCV_API.BAL.Services.Website.Activities;
using DPCV_API.Models.ActivityModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website.Activity
{
    [Route("api/activities")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActivityDTO>>> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDTO>> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
                return NotFound(new { message = "Activity not found" });

            return Ok(activity);
        }

        [HttpPost]
        public async Task<ActionResult> CreateActivity([FromBody] ActivityDTO activity)
        {
            bool isCreated = await _activityService.CreateActivityAsync(activity);
            return isCreated ? Ok() : StatusCode(500, new { message = "Error creating activity." });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateActivity(int id, [FromBody] ActivityDTO activity)
        {
            bool isUpdated = await _activityService.UpdateActivityAsync(id, activity);
            return isUpdated ? NoContent() : NotFound(new { message = "Activity not found or no changes made." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(int id)
        {
            bool isDeleted = await _activityService.DeleteActivityAsync(id);
            return isDeleted ? NoContent() : NotFound(new { message = "Activity not found." });
        }
    }
}
