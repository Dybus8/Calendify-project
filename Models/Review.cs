namespace StarterKit.Models
{
    public class Review
    {
        public int Id { get; set; }           // Identifier for Review
        public int EventId { get; set; }      // Foreign key reference to Event
        public int UserId { get; set; }    // User identifier
        public int Rating { get; set; }        // Rating value
        public string Comment { get; set; }    // User's comment
        public DateTime CreatedDate { get; set; } // Optional: Date when review was created


    }
}