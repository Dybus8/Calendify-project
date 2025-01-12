using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using Microsoft.EntityFrameworkCore;

namespace StarterKit.Services
{
    public class LoginService : ILoginService
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoginService> _logger;

        public LoginService(
            DatabaseContext context, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public LoginStatus Login(string email, string password, bool isAdminLogin = false)
        {
            try
            {
                var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Email == email);
                if (userAccount == null)
                    return LoginStatus.IncorrectUsername;

                string hashedPassword = EncryptionHelper.EncryptPassword(password);
                if (userAccount.Password != hashedPassword)
                    return LoginStatus.IncorrectPassword;

                SetUserSession(userAccount);
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
            // Check if user already exists
            var existingUser = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == registrationDto.Email);

            if (existingUser != null)
                throw new Exception("Email already exists");

            var newUser = new UserAccount
            {
                UserName = registrationDto.UserName, // Use the UserName from the DTO
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Email = registrationDto.Email,
                Password = EncryptionHelper.EncryptPassword(registrationDto.Password),
                IsAdmin = false, // Default to false for regular users
                RecuringDays = "", // Default or from DTO
                Attendances = new List<Attendance>(),
                Event_Attendances = new List<Event_Attendance>()
            };

            _context.UserAccounts.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public UserAccount? GetLoggedInUser()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userId = session?.GetInt32("UserId");

            if (!userId.HasValue) return null;

            return _context.UserAccounts
                .Include(u => u.Attendances)
                .Include(u => u.Event_Attendances)
                .FirstOrDefault(u => u.Id == userId.Value);
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
            return await _context.UserAccounts.ToListAsync();
        }
    }
}


