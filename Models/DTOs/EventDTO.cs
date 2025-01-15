using System;
using System.Collections.Generic;

namespace StarterKit.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } // Changed from DateOnly to DateTime
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Location { get; set; }
        public int Points { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }
        public List<AttendeeDTO>? Attendees { get; set; }
        public bool AdminApproval { get; set; }
    }
}
