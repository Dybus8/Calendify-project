namespace StarterKit.Models
{
    public class UserAccount
    {
        public required int Id { get; set; } // Can be used for both AdminId and UserId
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required bool IsAdmin { get; set; } // New property to differentiate

        // User-specific properties
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string RecuringDays { get; set; }
        public required List<Attendance> Attendances { get; set; }
        public required List<Event_Attendance> Event_Attendances { get; set; }
    }
}
