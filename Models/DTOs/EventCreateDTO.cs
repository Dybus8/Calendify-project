using System;

namespace StarterKit.Models.DTOs
{
    public class EventCreateDTO
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public required string Date { get; set; }  // Accept date as string
        public required string StartTime { get; set; }  // Accept time as string
        public required string EndTime { get; set; }    // Accept time as string
        public int Points { get; set; }
    }
}