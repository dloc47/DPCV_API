using DPCV_API.BAL.Services.Website.SearchFilter;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website.SearchFilter
{
    [Route("api/filter")]
    [ApiController]
    public class SearchfilterController : ControllerBase
    {
        private readonly ISearchfilterService _filterService;
        private readonly ILogger<SearchfilterController> _logger;

        public SearchfilterController(ISearchfilterService filterService, ILogger<SearchfilterController> logger)
        {
            _filterService = filterService;
            _logger = logger;
        }

        [HttpGet("get-filtered-data")]
        public async Task<IActionResult> GetFilteredData(
            [FromQuery] string category,
            [FromQuery] int? districtId,
            [FromQuery] int? villageId,
            [FromQuery] string? search)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest(new { message = "Category is required." });

            try
            {
                var data = await _filterService.GetFilteredDataAsync(category, districtId, villageId, search);

                if (data.Count == 0)
                    return NotFound(new { message = "No records found." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching filtered data.");
                return StatusCode(500, new { message = "An error occurred while processing the request." });
            }
        }
    }
}
