using System;

namespace StarterKit.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
    }
}