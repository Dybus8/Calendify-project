namespace StarterKit.Models.DTOs
{
    public class UserLoginResultDTO
    {
        public StarterKit.Utils.LoginStatus Status { get; set; }
        public UserAccount? User { get; set; }
    }

    public enum LoginStatus
    {
        Success,
        UserNotFound,
        InvalidPassword,
        Unknown
    }
}
