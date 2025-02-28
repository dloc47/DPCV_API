using System.Security.Claims;
using DPCV_API.Models.ActivityModel;

namespace DPCV_API.BAL.Services.Website.Activities
{
    public interface IActivityService
    {
        Task<List<ActivityDTO>> GetAllActivitiesAsync();
        Task<ActivityDTO?> GetActivityByIdAsync(int activityId);
        Task<bool> CreateActivityAsync(ActivityDTO activity, ClaimsPrincipal user);
        Task<bool> UpdateActivityAsync(int activityId, ActivityDTO activity, ClaimsPrincipal user);
        Task<bool> DeleteActivityAsync(int activityId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveActivityAsync(int activityId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveActivityAsync(int activityId, ClaimsPrincipal user);

    }
}
