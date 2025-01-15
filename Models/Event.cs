namespace StarterKit.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Points { get; set; }
        public bool AdminApproval { get; set; }
        public virtual ICollection<Event_Attendance> Event_Attendances { get; set; } = new List<Event_Attendance>(); // Initialize
    }
}
