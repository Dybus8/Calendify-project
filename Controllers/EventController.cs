using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _eventService.GetEvents();
            return Ok(events);
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventModel model)
        {
            if (!HttpContext.Session.GetString("UserRole").Equals("Admin"))
            {
                return Unauthorized(new { message = "Only admins can create events" });
            }

            var result = _eventService.CreateEvent(model);
            if (result.Success)
            {
                return Ok(new { message = "Event created successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateEvent(int eventId, [FromBody] EventModel model)
        {
            if (!HttpContext.Session.GetString("UserRole").Equals("Admin"))
            {
                return Unauthorized(new { message = "Only admins can update events" });
            }

            var result = _eventService.UpdateEvent(eventId, model);
            if (result.Success)
            {
                return Ok(new { message = "Event updated successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpDelete("{eventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            if (!HttpContext.Session.GetString("UserRole").Equals("Admin"))
            {
                return Unauthorized(new { message = "Only admins can delete events" });
            }

            var result = _eventService.DeleteEvent(eventId);
            if (result.Success)
            {
                return Ok(new { message = "Event deleted successfully" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}