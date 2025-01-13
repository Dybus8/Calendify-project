using StarterKit.Models;
using StarterKit.Models.DTOs;
using System.Threading.Tasks;

namespace StarterKit.Services
{
    public interface ILoginService
    {
        Task<UserLoginResultDTO> LoginUserAsync(UserLoginDTO loginDto);
        Task<UserAccount> RegisterUser(UserRegistrationDTO registrationDto);
        Task<UserSessionInfo> GetCurrentUserSessionInfoAsync();
        bool IsLoggedIn();
        bool IsAdmin();
        void Logout();
        Task<List<UserAccount>> GetAllUsersAsync();
        Task<UserAccount> GetUserByUserNameAsync(string username);
    }
}
