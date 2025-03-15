using System.Security.Claims;
using DPCV_API.Models.ActivityModel;
using DPCV_API.Models.CommonModel;

namespace DPCV_API.BAL.Services.Activities
{
    public interface IActivityService
    {
        Task<PaginatedResponse<ActivityResponseDTO>> GetPaginatedActivitiesAsync(int pageNumber, int pageSize);
        Task<List<ActivityResponseDTO>> GetAllActivitiesAsync();
        Task<ActivityResponseDTO?> GetActivityByIdAsync(int activityId);
        Task<bool> CreateActivityAsync(ActivityDTO activity, ClaimsPrincipal user);
        Task<bool> UpdateActivityAsync(int activityId, ActivityDTO activity, ClaimsPrincipal user);
        Task<bool> DeleteActivityAsync(int activityId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveActivityAsync(int activityId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveActivityAsync(int activityId, ClaimsPrincipal user);

    }
}
