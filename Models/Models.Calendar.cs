namespace StarterKit.Models
{
	public class User
	{
		public int UserId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public required string Email { get; set; }

		public required string Password { get; set; }

		// A comma separated string that could look like this: "mo,tu,we,th,fr"
		public string RecuringDays { get; set; }

		public List<Attendance> Attendances { get; set; }

		public List<Event_Attendance> Event_Attendances { get; set; }
		public required string Username { get; set; }
		public required string Role { get; set; }
	}

	public class Attendance
	{
		public int AttendanceId { get; set; }

		public DateTime AttendanceDate { get; set; }

		public required int UserId { get; set; }
		public int EventId { get; internal set; }
		
		public Attendance(int userId, int eventId)
		{
			UserId = userId;
			EventId = eventId;
		}
	}

	public class Event_Attendance
	{
		public int Event_AttendanceId { get; set; }
		public int Rating { get; set; }
		public required string Feedback { get; set; }
		public required User User { get; set; }
		public required Event Event { get; set; }
	}

	public class Event
	{
		public int EventId { get; set; }

		public required string Title { get; set; }

		public required string Description { get; set; }

		public DateTime EventDate { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan EndTime { get; set; }

		public required string Location { get; set; }

		public bool AdminApproval { get; set; }

		public List<Event_Attendance> Event_Attendances { get; set; }
		public object Reviews { get; internal set; }
		public object Attendees { get; internal set; }
	}
}