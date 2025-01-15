using System;

namespace StarterKit.Models.DTOs
{
    public class EventUpdateDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly EventDate { get; set; } // Changed from DateTime to DateOnly
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Location { get; set; }
    }
}
