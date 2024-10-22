using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public IActionResult AttendEvent([FromBody] AttendanceModel model)
        {
            var result = _attendanceService.AttendEvent(model);
            if (result.Success)
            {
                return Ok(new { message = "Attendance registered successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpGet]
        public IActionResult GetAttendees(int eventId)
        {
            var attendees = _attendanceService.GetAttendees(eventId);
            return Ok(attendees);
        }

        [HttpDelete]
        public IActionResult CancelAttendance(int eventId)
        {
            var result = _attendanceService.CancelAttendance(eventId);
            if (result.Success)
            {
                return Ok(new { message = "Attendance cancelled successfully" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}