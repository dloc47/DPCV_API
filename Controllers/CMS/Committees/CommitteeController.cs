using System.Diagnostics;
using DPCV_API.BAL.Services.Committees;
using DPCV_API.Models.CommitteeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website
{
    [Route("api/committees")]
    [ApiController]
    public class CommitteeController : ControllerBase
    {
        private readonly ICommitteeService _committeeService;
        private readonly ILogger<CommitteeController> _logger;

        public CommitteeController(ICommitteeService committeeService, ILogger<CommitteeController> logger)
        {
            _committeeService = committeeService;
            _logger = logger;
        }

        [HttpGet("villages")]
        public async Task<IActionResult> GetAllVillageNames()
        {
            var result = await _committeeService.GetAllVillageNamesAsync();
            return Ok(result);
        }

        [HttpGet("paginated-committees")]
        public async Task<IActionResult> GetPaginatedCommittees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Invalid pagination parameters." });
                }

                var result = await _committeeService.GetPaginatedCommitteesAsync(pageNumber, pageSize);

                return result.Data.Any() ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated committees.");
                return StatusCode(500, new { message = "An error occurred while fetching committee data." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCommittees()
        {
            var result = await _committeeService.GetAllCommitteesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommitteeById(int id)
        {
            var result = await _committeeService.GetCommitteeByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Committee not found" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCommittee([FromBody] CommitteeDTO committeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _committeeService.CreateCommitteeAsync(committeeDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to create this Committee." });
            }

            return Ok(new { message = "Committee created successfully." });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommittee(int id, [FromBody] CommitteeDTO committeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            if (id != committeeDto.CommitteeId)
            {
                return BadRequest(new { message = "Committee ID mismatch." });
            }

            var result = await _committeeService.UpdateCommitteeAsync(committeeDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to update this Committee or no changes were made." });
            }

            return Ok(new { message = "Committee updated successfully." });
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommittee(int id)
        {
            var result = await _committeeService.DeleteCommitteeAsync(id, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to delete this Committee or it does not exist." });
            }

            return Ok(new { message = "Committee deleted successfully." });
        }

        // ✅ Archive Committee (Set is_active = 0)
        [HttpPut("{id}/archive")]
        [Authorize]
        public async Task<IActionResult> ArchiveCommittee(int id)
        {
            var (success, message) = await _committeeService.ArchiveCommitteeAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        // ✅ Unarchive Committee (Set is_active = 1)
        [HttpPut("{id}/unarchive")]
        [Authorize]
        public async Task<IActionResult> UnarchiveCommittee(int id)
        {
            var (success, message) = await _committeeService.UnarchiveCommitteeAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

    }
}
