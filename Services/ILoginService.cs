using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using System.Threading.Tasks;

namespace StarterKit.Services
{
    public interface ILoginService
    {
        Task<LoginStatus> Login(string username, string password, bool isAdminLogin = false);
        Task<UserAccount> RegisterUser(UserRegistrationDTO registrationDto);
        Task<UserSessionInfo> GetCurrentUserSessionInfoAsync();
        bool IsLoggedIn();
        bool IsAdmin();
        void Logout();
        Task<List<UserAccount>> GetAllUsersAsync();
        Task<UserAccount> GetUserByUserNameAsync(string username);
    }
}
