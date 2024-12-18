using StarterKit.Models.DTOs;
using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services
{
    public interface ILoginService
    {
        LoginStatus Login(string email, string password, bool isAdminLogin = false);
        void Logout();
        bool IsLoggedIn();
        bool IsAdmin();
        User? GetLoggedInUser();
        Task<User> RegisterUser(UserRegistrationDTO userDTO);
    }
}