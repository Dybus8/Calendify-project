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
                if (isAdminLogin)
                {
                    var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
                    if (admin == null)
                        return LoginStatus.IncorrectUsername;

                    string hashedPassword = EncryptionHelper.EncryptPassword(password);
                    if (admin.Password != hashedPassword)
                        return LoginStatus.IncorrectPassword;

                    SetAdminSession(admin);
                    return LoginStatus.Success;
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == email);
                    if (user == null)
                        return LoginStatus.IncorrectUsername;

                    string hashedPassword = EncryptionHelper.EncryptPassword(password);
                    if (user.Password != hashedPassword)
                        return LoginStatus.IncorrectPassword;

                    SetUserSession(user);
                    return LoginStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return LoginStatus.Unknown;
            }
        }

        private void SetUserSession(User user)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetInt32("UserId", user.UserId);
            session.SetString("Email", user.Email);
            session.SetString("FirstName", user.FirstName);
            session.SetString("LastName", user.LastName);
            session.SetString("Role", "User");
        }

        private void SetAdminSession(Admin admin)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetInt32("AdminId", admin.AdminId);
            session.SetString("Email", admin.Email);
            session.SetString("Role", "Admin");
        }

        public async Task<User> RegisterUser(UserRegistrationDTO registrationDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registrationDto.Email);

            if (existingUser != null)
                throw new Exception("Email already exists");

            var newUser = new User
            {
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Email = registrationDto.Email,
                Password = EncryptionHelper.EncryptPassword(registrationDto.Password),
                RecuringDays = "", // Default or from DTO
                Attendances = new List<Attendance>(),
                Event_Attendances = new List<Event_Attendance>()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public User? GetLoggedInUser()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userId = session?.GetInt32("UserId");

            if (!userId.HasValue) return null;

            return _context.Users
                .Include(u => u.Attendances)
                .Include(u => u.Event_Attendances)
                .FirstOrDefault(u => u.UserId == userId.Value);
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
    }
}