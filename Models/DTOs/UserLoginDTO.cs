// Models/DTOs/UserLoginDTO.cs
namespace StarterKit.Models.DTOs
{
    public class UserLoginDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}