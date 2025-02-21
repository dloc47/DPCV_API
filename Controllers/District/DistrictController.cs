using DPCV_API.Models.DistrictModel;
using DPCV_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers
{
    [Route("api/districts")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        // ✅ Get All Districts
        [HttpGet]
        public async Task<IActionResult> GetAllDistricts()
        {
            var districts = await _districtService.GetAllDistrictsAsync();
            return districts.Count > 0 ? Ok(districts) : NoContent();
        }

        // ✅ Get District by ID
        [HttpGet("{id}", Name = "GetDistrictById")]
        public async Task<IActionResult> GetDistrictById(int id)
        {
            var district = await _districtService.GetDistrictByIdAsync(id);
            return district is not null ? Ok(district) : NotFound(new { message = "District not found" });
        }

        // ✅ Create District
        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictDTO district)
        {
            if (district is null || string.IsNullOrWhiteSpace(district.DistrictName) || string.IsNullOrWhiteSpace(district.Region))
            {
                return BadRequest(new { message = "District name and region are required." });
            }

            bool isCreated = await _districtService.CreateDistrictAsync(district);
            return isCreated
                ? CreatedAtRoute("GetDistrictById", new { id = district.DistrictId }, district)
                : StatusCode(500, new { message = "Error creating district." });
        }

        // ✅ Update District
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(int id, [FromBody] DistrictDTO district)
        {
            if (district is null)
            {
                return BadRequest(new { message = "Invalid district data." });
            }

            bool isUpdated = await _districtService.UpdateDistrictAsync(id, district);
            return isUpdated ? NoContent() : NotFound(new { message = "District not found or no changes were made." });
        }
    }
}
