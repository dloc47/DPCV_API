using DPCV_API.BAL.Services.Districts;
using DPCV_API.Models.Website.DistrictModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers
{
    [ApiController]
    [Route("api/districts")]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        private readonly ILogger<DistrictController> _logger;

        public DistrictController(IDistrictService districtService, ILogger<DistrictController> logger)
        {
            _districtService = districtService;
            _logger = logger;
        }

        // ✅ Get All Districts
        [HttpGet]
        public async Task<IActionResult> GetAllDistricts()
        {
            var districts = await _districtService.GetAllDistrictsAsync();
            if (districts.Count == 0)
            {
                _logger.LogWarning("No districts found.");
                return NotFound("No districts available.");
            }
            return Ok(districts);
        }

        // ✅ Get District by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrictById(int id)
        {
            var district = await _districtService.GetDistrictByIdAsync(id);
            if (district == null)
            {
                _logger.LogWarning("District with ID {DistrictId} not found.", id);
                return NotFound($"District with ID {id} not found.");
            }
            return Ok(district);
        }

        // ✅ Create District (Admin Only)
        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictDTO district)
        {
            if (district == null || string.IsNullOrWhiteSpace(district.DistrictName) || string.IsNullOrWhiteSpace(district.Region))
            {
                return BadRequest("Invalid district data.");
            }

            var user = HttpContext.User;
            bool isCreated = await _districtService.CreateDistrictAsync(district, user);
            if (!isCreated)
            {
                _logger.LogWarning("Unauthorized or failed to create district: {DistrictName}", district.DistrictName);
                return Forbid();
            }

            _logger.LogInformation("District created successfully: {DistrictName}", district.DistrictName);
            return CreatedAtAction(nameof(GetDistrictById), new { id = district.DistrictId }, district);
        }

        // ✅ Update District (Admin Only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(int id, [FromBody] DistrictDTO district)
        {
            if (district == null || id != district.DistrictId)
            {
                return BadRequest("Invalid district data.");
            }

            var user = HttpContext.User;
            bool isUpdated = await _districtService.UpdateDistrictAsync(district, user);
            if (!isUpdated)
            {
                _logger.LogWarning("Unauthorized or failed to update district: {DistrictId}", id);
                return Forbid();
            }

            _logger.LogInformation("District updated successfully: {DistrictId}", id);
            return Ok("District updated successfully.");
        }
    }
}