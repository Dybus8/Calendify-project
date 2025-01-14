using System;

namespace StarterKit.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ReviewCreateRequestDTO
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}