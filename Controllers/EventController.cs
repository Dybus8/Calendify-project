using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Models.DTOs;
using StarterKit.Services;
using System;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
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
                var updatedEvent = await _eventService.AttendEventAsync(eventId, userId);

                return Ok(updatedEvent);
            }
            catch (EventAttendanceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
