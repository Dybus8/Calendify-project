namespace StarterKit.Models
{
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

        public required List<Event_Attendance> Event_Attendances { get; set; }
    }
}