﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DPCV_API.BAL.Services.Events;
using DPCV_API.Models.EventModel;

namespace DPCV_API.Controllers.Website
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

        // ✅ Get Paginated Events
        [HttpGet("paginated-events")]
        public async Task<IActionResult> GetPaginatedEvents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Invalid pagination parameters." });
                }

                var result = await _eventService.GetPaginatedEventsAsync(pageNumber, pageSize);

                return result.Data.Any() ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated events.");
                return StatusCode(500, new { message = "An error occurred while fetching event data." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var result = await _eventService.GetAllEventsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Event not found" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDTO eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _eventService.CreateEventAsync(eventDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to create this event." });
            }

            return Ok(new { message = "Event created successfully." });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDTO eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _eventService.UpdateEventAsync(eventDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to update this event or no changes were made." });
            }

            return Ok(new { message = "Event updated successfully." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to delete this event or it does not exist." });
            }

            return Ok(new { message = "Event deleted successfully." });
        }

        [Authorize]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveEvent(int id)
        {
            var (success, message) = await _eventService.ArchiveEventAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnarchiveEvent(int id)
        {
            var (success, message) = await _eventService.UnarchiveEventAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }
    }
}
