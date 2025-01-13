using System.Text.Json.Serialization;

namespace StarterKit.Models.DTOs
{
    public class LoginDTO
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        public LoginDTO() { }

        public LoginDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
