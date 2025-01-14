using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Repositories; 
using StarterKit.Utils;
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

		// Constructor to initialize the repositories
		public AttendanceController(IAttendanceRepository attendanceRepository, IUserRepository userRepository)
		{
			_attendanceRepository = attendanceRepository;
			_userRepository = userRepository;
		}

		// Endpoint to book attendance
		[Authorize]
		[HttpPost("attend")]
		public async Task<IActionResult> BookAttendance([FromBody] OfficeAttendanceDTO attendanceDto)
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

			 // Check if the event exists
			var eventEntity = await _attendanceRepository.GetEventByIdAsync(attendanceDto.EventId);
			if (eventEntity == null)
			{
				return NotFound("Event not found.");
			}

			// Create a new attendance record
			var attendance = new Attendance
			{
				User = user,
				Event = eventEntity,
				AttendanceDate = attendanceDto.Date,
			};
			attendance.User.Id = userId;
			await _attendanceRepository.AddAttendanceAsync(attendance);
			return Ok("Attendance booked successfully.");
		}

		// Endpoint to update attendance
		[Authorize]
		[HttpPut("update")]
		public async Task<IActionResult> UpdateAttendance([FromBody] OfficeAttendanceDTO attendanceDto)
		{
			// Get the logged-in user's ID from the claims
			var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userId == null)
			{
				return Unauthorized("User not logged in.");
			}

			// Get the attendance record by ID
			var attendance = await _attendanceRepository.GetAttendanceByIdAsync(attendanceDto.UserId);
			if (attendance == null || attendance.User.Id != int.Parse(userId))
			{
				return NotFound("Attendance not found or you do not have permission to update it.");
			}

			// Update the attendance date
			attendance.AttendanceDate = attendanceDto.Date;
			await _attendanceRepository.UpdateAttendanceAsync(attendance);
			return Ok("Attendance updated successfully.");
		}

		// Endpoint to remove attendance
		[Authorize]
		[HttpDelete("remove")]
		public async Task<IActionResult> RemoveAttendance([FromBody] OfficeAttendanceDTO attendanceDto)
		{
			// Get the logged-in user's ID from the claims
			var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userId == null)
			{
				return Unauthorized("User not logged in.");
			}

			// Get the attendance record by date and user ID
			var attendance = await _attendanceRepository.GetAttendanceByDateAndUserIdAsync(attendanceDto.Date, int.Parse(userId));
			if (attendance == null)
			{
				return NotFound("You're not attending this .");
			}

			// Remove the attendance record
			await _attendanceRepository.DeleteAttendanceAsync(attendance.AttendanceId);
			return Ok("Attendance removed successfully.");
		}
	}
}