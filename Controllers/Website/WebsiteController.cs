using Microsoft.AspNetCore.Mvc;
using DPCV_API.BAL.Services.Website.Committees;
using DPCV_API.BAL.Services.Website.EntityCount;
using DPCV_API.BAL.Services.Website.Events;
using DPCV_API.BAL.Services.Website.Products;
using DPCV_API.BAL.Services.Website.SearchFilter;
using DPCV_API.BAL.Services.Website.Activities;
using DPCV_API.BAL.Services.Website.Homestays;
using DPCV_API.Services;

[Route("api/website")]
[ApiController]
public class WebsiteController : ControllerBase
{
    private readonly ICommitteeService _committeeService;
    private readonly IEntityService _entityService;
    private readonly IEventService _eventService;
    private readonly IHomestayService _homestayService;
    private readonly IProductService _productService;
    private readonly ISearchfilterService _filterService;
    private readonly IDistrictService _districtService;
    private readonly IActivityService _activityService;
    private readonly ILogger<WebsiteController> _logger;

    public WebsiteController(
        ICommitteeService committeeService,
        IEntityService entityService,
        IEventService eventService,
        IHomestayService homestayService,
        IProductService productService,
        ISearchfilterService filterService,
        IDistrictService districtService,
        IActivityService activityService,
        ILogger<WebsiteController> logger)
    {
        _committeeService = committeeService;
        _entityService = entityService;
        _eventService = eventService;
        _homestayService = homestayService;
        _productService = productService;
        _filterService = filterService;
        _districtService = districtService;
        _activityService = activityService;
        _logger = logger;
    }

    // ✅ Get Entity Counts
    [HttpGet("entity-counts")]
    public async Task<IActionResult> GetEntityCounts()
    {
        try
        {
            var counts = await _entityService.GetEntityCountsAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching entity counts.");
            return StatusCode(500, new { message = "An error occurred. Please try again later." });
        }
    }

    // ✅ Get All Districts
    [HttpGet("districts")]
    public async Task<IActionResult> GetAllDistricts()
    {
        try
        {
            var districts = await _districtService.GetAllDistrictsAsync();
            return districts.Any() ? Ok(districts) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching districts.");
            return StatusCode(500, new { message = "An error occurred while fetching districts." });
        }
    }

    // ✅ Get District by ID
    [HttpGet("district/{id}")]
    public async Task<IActionResult> GetDistrictById(int id)
    {
        try
        {
            var district = await _districtService.GetDistrictByIdAsync(id);
            return district != null ? Ok(district) : NotFound(new { message = "District not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching district with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the district." });
        }
    }

    // ✅ Get All Villages
    [HttpGet("villages")]
    public async Task<IActionResult> GetAllVillages()
    {
        try
        {
            var villages = await _committeeService.GetAllVillageNamesAsync();
            return villages.Any() ? Ok(villages) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching village names.");
            return StatusCode(500, new { message = "An error occurred while fetching village names." });
        }
    }

    // ✅ Get All Committees
    [HttpGet("committees")]
    public async Task<IActionResult> GetAllCommittees()
    {
        try
        {
            var committees = await _committeeService.GetAllCommitteesAsync();
            return committees.Any() ? Ok(committees) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching committees.");
            return StatusCode(500, new { message = "An error occurred while fetching committees." });
        }
    }

    // ✅ Get Committee by ID
    [HttpGet("committee/{id}")]
    public async Task<IActionResult> GetCommitteeById(int id)
    {
        try
        {
            var committee = await _committeeService.GetCommitteeByIdAsync(id);
            return committee != null ? Ok(committee) : NotFound(new { message = "Committee not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching committee with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the committee." });
        }
    }


    // ✅ Get All Homestays
    [HttpGet("homestays")]
    public async Task<IActionResult> GetAllHomestays()
    {
        try
        {
            var homestays = await _homestayService.GetAllHomestaysAsync();
            return homestays.Any() ? Ok(homestays) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching homestays.");
            return StatusCode(500, new { message = "An error occurred while fetching homestays." });
        }
    }

    // ✅ Get Homestay by ID
    [HttpGet("homestay/{id}")]
    public async Task<IActionResult> GetHomestayById(int id)
    {
        try
        {
            var homestay = await _homestayService.GetHomestayByIdAsync(id);
            return homestay != null ? Ok(homestay) : NotFound(new { message = "Homestay not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching homestay with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the homestay." });
        }
    }

    // ✅ Get All Products
    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return products.Any() ? Ok(products) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products.");
            return StatusCode(500, new { message = "An error occurred while fetching products." });
        }
    }

    // ✅ Get Product by ID
    [HttpGet("product/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null ? Ok(product) : NotFound(new { message = "Product not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching product with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the product." });
        }
    }

    // ✅ Get All Activities
    [HttpGet("activities")]
    public async Task<IActionResult> GetAllActivities()
    {
        try
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return activities.Any() ? Ok(activities) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching activities.");
            return StatusCode(500, new { message = "An error occurred while fetching activities." });
        }
    }

    // ✅ Get Activity by ID
    [HttpGet("activity/{id}")]
    public async Task<IActionResult> GetActivityById(int id)
    {
        try
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            return activity != null ? Ok(activity) : NotFound(new { message = "Activity not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching activity with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the activity." });
        }
    }

    // ✅ Get All Events
    [HttpGet("events")]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync();
            return events.Any() ? Ok(events) : NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events.");
            return StatusCode(500, new { message = "An error occurred while fetching events." });
        }
    }

    // ✅ Get Event by ID
    [HttpGet("event/{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        try
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            return eventDto != null ? Ok(eventDto) : NotFound(new { message = "Event not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching event with ID {id}");
            return StatusCode(500, new { message = "An error occurred while fetching the event." });
        }
    }

    // ✅ Search & Filter API
    [HttpGet("filter")]
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
            return data.Count > 0 ? Ok(data) : NotFound(new { message = "No records found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching filtered data.");
            return StatusCode(500, new { message = "An error occurred while processing the request." });
        }
    }
}
