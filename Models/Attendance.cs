namespace StarterKit.Models
{
	public class Attendance
	{
		public int AttendanceId { get; set; }
		public DateTime AttendanceDate { get; set; }
		public required UserAccount User { get; set; }
		public string? Notes { get; set; }

    }
}
