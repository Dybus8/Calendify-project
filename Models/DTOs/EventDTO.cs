
using System;
using System.Collections.Generic;

namespace StarterKit.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Location { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }
        public List<AttendeeDTO>? Attendees { get; set; }
    }
}