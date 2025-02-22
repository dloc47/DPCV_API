using DPCV_API.BAL.Services.Website.Events;
using DPCV_API.Models.Website.EventModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website.Event
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
                return NotFound(new { message = "Event not found." });

            return Ok(eventDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDTO eventDto)
        {
            if (eventDto == null)
                return BadRequest(new { message = "Invalid event data." });

            bool created = await _eventService.CreateEventAsync(eventDto);
            if (!created)
                return BadRequest(new { message = "Event creation failed." });

            return Ok(new { message = "Event created successfully." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDTO eventDto)
        {
            if (eventDto == null)
                return BadRequest(new { message = "Invalid event data." });

            bool updated = await _eventService.UpdateEventAsync(id, eventDto);
            if (!updated)
                return NotFound(new { message = "Event not found or update failed." });

            return Ok(new { message = "Event updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            bool deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted)
                return NotFound(new { message = "Event not found or delete failed." });

            return Ok(new { message = "Event deleted successfully." });
        }
    }
}
