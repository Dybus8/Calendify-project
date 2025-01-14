namespace StarterKit.Models
{
	public class Event_Attendance
	{
		public int Event_AttendanceId { get; set; }
		public int Rating { get; set; }
		public required string Feedback { get; set; }
		public required UserAccount UserAccount { get; set; } // Updated to UserAccount
		public required Event Event { get; set; }
		public int Id { get; internal set; }
		public int EventId { get; internal set; }
		public required int Points { get; set; }
	}
}
