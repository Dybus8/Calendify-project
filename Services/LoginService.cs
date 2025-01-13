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

        public async Task<UserLoginResultDTO> LoginUserAsync(UserLoginDTO loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Login attempt with empty username or password");
                    return new UserLoginResultDTO { Status = StarterKit.Utils.LoginStatus.InvalidPassword };
                }

                var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", loginDto.Username);
                    return new UserLoginResultDTO { Status = StarterKit.Utils.LoginStatus.UserNotFound };
                }

                if (!EncryptionHelper.VerifyPassword(loginDto.Password, user.Password))
                {
                    _logger.LogWarning("Invalid password for user: {Username", loginDto.Username);
                    return new UserLoginResultDTO { Status = StarterKit.Utils.LoginStatus.InvalidPassword };
                }

                SetUserSession(user);
                _logger.LogInformation("User {Username} logged in successfully", loginDto.Username);
                return new UserLoginResultDTO { Status = StarterKit.Utils.LoginStatus.Success, User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", loginDto.Username);
                return new UserLoginResultDTO { Status = StarterKit.Utils.LoginStatus.Unknown };
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
                throw new Exception("Username already exists");
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
