using DPCV_API.BAL.Services.Website.EntityCount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/entities")]
public class EntityController : ControllerBase
{
    private readonly IEntityService _entityService;
    private readonly ILogger<EntityController> _logger;

    public EntityController(IEntityService entityService, ILogger<EntityController> logger)
    {
        _entityService = entityService;
        _logger = logger;
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetEntityCounts()
    {
        try
        {
            var counts = await _entityService.GetEntityCountsAsync();
            return Ok(counts);
        }
        catch (ApplicationException ex)
        {
            _logger.LogWarning(ex.Message);
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching entity counts.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}
