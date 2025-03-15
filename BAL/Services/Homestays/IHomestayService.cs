using System.Security.Claims;
using DPCV_API.Models.CommonModel;
using DPCV_API.Models.HomestayModel;

namespace DPCV_API.BAL.Services.Homestays
{
    public interface IHomestayService
    {
        Task<PaginatedResponse<HomestayResponseDTO>> GetPaginatedHomestaysAsync(int pageNumber, int pageSize);
        Task<List<HomestayResponseDTO>> GetAllHomestaysAsync();
        Task<HomestayResponseDTO?> GetHomestayByIdAsync(int homestayId);
        Task<bool> CreateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user);
        Task<bool> UpdateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user);
        Task<bool> DeleteHomestayAsync(int homestayId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveHomestayAsync(int homestayId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveHomestayAsync(int homestayId, ClaimsPrincipal user);
    }

}
