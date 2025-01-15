using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Repositories; 
using StarterKit.Utils;
using StarterKit.Services; // Added this line
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventService _eventService;

        // Constructor to initialize the repositories
        public AttendanceController(IAttendanceRepository attendanceRepository, IUserRepository userRepository, IEventService eventService)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _eventService = eventService;
        }

        // Endpoint to book attendance
        [Authorize]
        [HttpPost("attend")]
        public async Task<IActionResult> BookAttendance([FromBody] AttendEventDTO attendanceDto)
        {
            // Get the logged-in user's ID from the claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in.");
            }
            int userId;
            if (!int.TryParse(userIdClaim, out userId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Get the user from the repository
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create an Event_Attendance record
            var attendEventDto = new AttendEventDTO
            {
                EventId = attendanceDto.EventId,
                UserId = userId
            };

            await _eventService.AttendEventAsync(attendEventDto);
            return Ok("Attendance booked successfully.");
        }

        // Other methods remain unchanged...
    }
}
