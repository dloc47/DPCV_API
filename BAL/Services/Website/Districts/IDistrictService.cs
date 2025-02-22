using System.Collections.Generic;
using System.Threading.Tasks;
using DPCV_API.Models;
using DPCV_API.Models.Website.DistrictModel;

namespace DPCV_API.Services
{
    public interface IDistrictService
    {
        Task<List<DistrictDTO>> GetAllDistrictsAsync();
        Task<DistrictDTO?> GetDistrictByIdAsync(int districtId);
        Task<bool> CreateDistrictAsync(DistrictDTO district);
        Task<bool> UpdateDistrictAsync(int districtId, DistrictDTO district);
    }
}
