using System.Security.Claims;
using DPCV_API.Models.Website.DistrictModel;

namespace DPCV_API.Services
{
    public interface IDistrictService
    {
        Task<List<DistrictDTO>> GetAllDistrictsAsync();
        Task<DistrictDTO?> GetDistrictByIdAsync(int districtId);
        Task<bool> CreateDistrictAsync(DistrictDTO district, ClaimsPrincipal user);
        Task<bool> UpdateDistrictAsync(DistrictDTO district, ClaimsPrincipal user);
    }
}
