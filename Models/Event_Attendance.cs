namespace StarterKit.Models
{
    public class Event_Attendance
    {
        public int Event_AttendanceId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? Feedback { get; set; } // Marked as nullable
        public int Points { get; set; } // Default value can be set in the constructor

        public virtual UserAccount? UserAccount { get; set; } // Marked as nullable
        public virtual Event? Event { get; set; } // Marked as nullable
    }
}
