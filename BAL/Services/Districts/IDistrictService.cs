using System.Security.Claims;
using DPCV_API.Models.DistrictModel;

namespace DPCV_API.BAL.Services.Districts
{
    public interface IDistrictService
    {
        Task<List<DistrictDTO>> GetAllDistrictsAsync();
        Task<DistrictDTO?> GetDistrictByIdAsync(int districtId);
        Task<bool> CreateDistrictAsync(DistrictDTO district, ClaimsPrincipal user);
        Task<bool> UpdateDistrictAsync(DistrictDTO district, ClaimsPrincipal user);
    }
}
