using StarterKit.Models;

namespace StarterKit.Services
{
	public class AttendanceService : IAttendanceService
	{
		private readonly DatabaseContext _context;

		public AttendanceService(DatabaseContext context)
		{
			_context = context;
		}

		public AttendanceResult AttendEvent(AttendanceModel model)
		{
			var user = _context.User.Find(model.UserId);
			if (user == null)
			{
				return new AttendanceResult { Success = false, Message = "User  not found" };
			}

			var eventEntity = _context.Event.Find(model.EventId);
			if (eventEntity == null)
			{
				return new AttendanceResult { Success = false, Message = "Event not found" };
			}

			if (eventEntity.EventDate < DateTime.Now || eventEntity.StartTime < DateTime.Now.TimeOfDay)
			{
				return new AttendanceResult { Success = false, Message = "Event has already started or ended" };
			}

			var attendance = new Attendance
			{
				User = model.UserId,
				EventId = model.EventId
			};

			_context.Attendance.Add(attendance);
			_context.SaveChanges();

			return new AttendanceResult { Success = true };
		}

		public List<User> GetAttendees(int eventId)
		{
			return _context.Attendance
				.Where(a => a.EventId == eventId)
				.Select(a => a.User)
				.ToList();
		}

		public AttendanceResult CancelAttendance(int eventId)
		{
			var attendance = _context.Attendance
				.FirstOrDefault(a => a.EventId == eventId && a.User == int.Parse(HttpContext.Session.GetString("UserId")));

			if (attendance == null)
			{
				return new AttendanceResult { Success = false, Message = "Attendance not found" };
			}

			_context.Attendance.Remove(attendance);
			_context.SaveChanges();

			return new AttendanceResult { Success = true };
		}
	}
}