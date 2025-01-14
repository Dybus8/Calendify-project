using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Services;
using Microsoft.Extensions.Logging;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            var events = await _eventService.GetEventsAsync();
            return Ok(events);
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int eventId)
        {
            try
            {
                var @event = await _eventService.GetEventByIdAsync(eventId);
                return Ok(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event with ID {EventId}", eventId);
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventCreateDTO eventCreateDto)
        {
            try
            {
                var createdEvent = await _eventService.CreateEventAsync(eventCreateDto);
                return CreatedAtAction(nameof(GetEventById), new { eventId = createdEvent.Id }, createdEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{eventId}")]
        public async Task<ActionResult<EventDTO>> UpdateEvent(int eventId, [FromBody] EventUpdateDTO eventUpdateDto)
        {
            try
            {
                var updatedEvent = await _eventService.UpdateEventAsync(eventId, eventUpdateDto);
                return Ok(updatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with ID {EventId}", eventId);
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            try
            {
                await _eventService.DeleteEventAsync(eventId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with ID {EventId}", eventId);
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{eventId}/attend")]
        public async Task<ActionResult<EventDTO>> AttendEvent(int eventId)
        {
            try
            {
                var user = HttpContext.User;
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User ID claim not found." });
                }
                var userId = int.Parse(userIdClaim.Value);

                var attendEventDto = new AttendEventDTO { EventId = eventId, UserId = userId };

                var isAvailable = await _eventService.CheckEventAvailabilityAsync(eventId, attendEventDto);
                if (!isAvailable)
                {
                    return BadRequest(new { message = "Event is not available for attendance." });
                }

                var updatedEvent = await _eventService.AttendEventAsync(attendEventDto);
                return Ok(updatedEvent);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access while attending event with ID {EventId}", eventId);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attending event with ID {EventId}", eventId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
