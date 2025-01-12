using System;

namespace StarterKit.Models.DTOs
{
    public class ReviewCreateDTO
    {
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}