namespace StarterKit.Models.DTOs
{
    public class OfficeAttendanceDTO
    {
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public int EventId { get; set; } // Added EventId property
    }
}
