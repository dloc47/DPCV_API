using System.Security.Claims;
using DPCV_API.Models.Website.HomestayModel;

namespace DPCV_API.BAL.Services.Homestays
{
    public interface IHomestayService
    {
        Task<List<HomestayDTO>> GetAllHomestaysAsync();
        Task<HomestayDTO?> GetHomestayByIdAsync(int homestayId);
        Task<bool> CreateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user);
        Task<bool> UpdateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user);
        Task<bool> DeleteHomestayAsync(int homestayId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveHomestayAsync(int homestayId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveHomestayAsync(int homestayId, ClaimsPrincipal user);
    }

}
