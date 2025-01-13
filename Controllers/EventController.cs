using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Models.DTOs;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
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
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{eventId}/reviews")]
        public async Task<ActionResult<ReviewDTO>> CreateReview(int eventId, [FromBody] ReviewCreateDTO reviewCreateDto)
        {
            try
            {
                var review = await _eventService.CreateReviewAsync(eventId, reviewCreateDto);
                return CreatedAtAction(nameof(GetEventById), new { eventId }, review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{eventId}/attend")]
        public async Task<ActionResult<EventDTO>> AttendEvent(int eventId, [FromBody] AttendEventDTO attendEventDto)
        {
            try
            {
                // Check event availability before allowing attendance
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
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{eventId}/attendees")]
        public async Task<ActionResult<IEnumerable<AttendeeDTO>>> GetEventAttendees(int eventId)
        {
            try
            {
                var attendees = await _eventService.GetEventAttendeesAsync(eventId);
                return Ok(attendees);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{eventId}/attendance")]
        public async Task<IActionResult> RemoveEventAttendance(int eventId)
        {
            try
            {
                var user = HttpContext.User;
                var userId = int.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                
                await _eventService.RemoveEventAttendanceAsync(eventId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // New endpoint for office attendance management
        [Authorize]
        [HttpPost("attendance")]
        public async Task<IActionResult> ManageOfficeAttendance([FromBody] OfficeAttendanceDTO officeAttendanceDto)
        {
            try
            {
                // Logic to manage office attendance
                await _eventService.ManageOfficeAttendanceAsync(officeAttendanceDto);
                return Ok(new { message = "Attendance recorded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
