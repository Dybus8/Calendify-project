namespace StarterKit.Models
{
    public class Review
    {
        public int Id { get; set; }           // Unique identifier for the review
        public int EventId { get; set; }      // Foreign key linking to Event
        public int UserId { get; set; }       // Foreign key linking to UserAccount
        public int Rating { get; set; }       // Rating between 1 and 10
        public string? Comment { get; set; }  // Optional review comment
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Review creation timestamp

        // Navigation properties
        public Event Event { get; set; } = null!;
        public UserAccount User { get; set; } = null!;
    }
}
