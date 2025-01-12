using System.ComponentModel.DataAnnotations;

namespace StarterKit.Models.DTOs
{
    public class LoginDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}