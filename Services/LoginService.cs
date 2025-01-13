using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using StarterKit.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StarterKit.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoginService> _logger;

        public LoginService(
            IUserRepository userRepository, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginService> logger)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<LoginStatus> Login(string username, string password, bool isAdminLogin = false)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return LoginStatus.InvalidPassword;
                }

                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return LoginStatus.UserNotFound;
                }

                if (!EncryptionHelper.VerifyPassword(password, user.Password))
                {
                    return LoginStatus.InvalidPassword;
                }

                SetUserSession(user);
                return LoginStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return LoginStatus.Unknown;
            }
        }

        private void SetUserSession(UserAccount userAccount)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetInt32("UserId", userAccount.Id);
            session.SetString("Email", userAccount.Email);
            session.SetString("FirstName", userAccount.FirstName);
            session.SetString("LastName", userAccount.LastName);
            session.SetString("Role", userAccount.IsAdmin ? "Admin" : "User");
        }

        public async Task<UserAccount> RegisterUser(UserRegistrationDTO registrationDto)
        {
            if (registrationDto == null)
            {
                throw new ArgumentNullException(nameof(registrationDto));
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(registrationDto.UserName);
            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            var newUser = new UserAccount
            {
                UserName = registrationDto.UserName,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Email = registrationDto.Email,
                Password = EncryptionHelper.EncryptPassword(registrationDto.Password),
                IsAdmin = false,
                RecuringDays = "",
                Attendances = new List<Attendance>(),
                Event_Attendances = new List<Event_Attendance>()
            };

            await _userRepository.AddUserAsync(newUser);
            return newUser;
        }

        public async Task<UserSessionInfo> GetCurrentUserSessionInfoAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                throw new InvalidOperationException("Session is not available");
            }

            var userId = session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                throw new KeyNotFoundException("User not logged in");
            }

            return new UserSessionInfo
            {
                UserId = userId.Value,
                Email = session.GetString("Email") ?? string.Empty,
                FirstName = session.GetString("FirstName") ?? string.Empty,
                LastName = session.GetString("LastName") ?? string.Empty,
                Role = session.GetString("Role") ?? "User"
            };
        }

        public bool IsLoggedIn()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString("Role") != null;
        }

        public bool IsAdmin()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString("Role") == "Admin";
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        public async Task<List<UserAccount>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<UserAccount> GetUserByUserNameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            return await _userRepository.GetUserByUsernameAsync(username);
        }
    }

    public class UserSessionInfo
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
