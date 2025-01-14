namespace StarterKit.Models.DTOs
{
	public class UserAccountDTO
	{
		public int UserId { get; set; }
		public required string Username { get; set; }
		public required string Email { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required int Points { get; set; }
	}
}