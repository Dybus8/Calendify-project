namespace StarterKit.Models
{
    public class UserAccount
    {
        public int Id { get; set; } // Removed 'required' to allow database to handle Id assignment
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required bool IsAdmin { get; set; } // New property to differentiate
        public int UserPoints { get; set; }

        // User-specific properties
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string RecuringDays { get; set; }
        public required List<Attendance> Attendances { get; set; }
        public required List<Event_Attendance> Event_Attendances { get; set; }
    }
}
