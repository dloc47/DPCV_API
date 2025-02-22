using DPCV_API.Models.ActivityModel;

namespace DPCV_API.BAL.Services.Website.Activities
{
    public interface IActivityService
    {
        Task<List<ActivityDTO>> GetAllActivitiesAsync();
        Task<ActivityDTO?> GetActivityByIdAsync(int activityId);
        Task<bool> CreateActivityAsync(ActivityDTO activity);
        Task<bool> UpdateActivityAsync(int activityId, ActivityDTO activity);
        Task<bool> DeleteActivityAsync(int activityId);
    }
}
