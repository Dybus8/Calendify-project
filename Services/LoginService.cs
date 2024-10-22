using StarterKit.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace StarterKit.Services
{
	public class LoginService : ILoginService
	{
		private readonly DatabaseContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public LoginService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		public LoginResult Login(string username, string password)
		{
			var user = _context.User.FirstOrDefault(u => u.Username == username);
			if (user == null || !VerifyPassword(password, user.Password))
			{
				return new LoginResult { Success = false, Message = "Invalid username or password" };
			}

			_httpContextAccessor.HttpContext.Session.SetString("UserId", user.UserId.ToString());
			_httpContextAccessor.HttpContext.Session.SetString("UserRole", user.Role);

			return new LoginResult { Success = true, User = user };
		}

		public LoginStatus GetLoginStatus()
		{
			var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
			if (string.IsNullOrEmpty(userId))
			{
				return new LoginStatus { IsLoggedIn = false };
			}

			var user = _context.User.Find(int.Parse(userId));
			return new LoginStatus { IsLoggedIn = true, Username = user.Username };
		}

		public RegistrationResult Register(RegisterModel model)
		{
			if (_context.User.Any(u => u.Username == model.Username))
			{
				return new RegistrationResult { Success = false, Message = "Username already exists" };
			}

			var user = new User
			{
				Username = model.Username,
				Password = HashPassword(model.Password),
				Email = model.Email,
				Role = "User"
			};

			_context.User.Add(user);
			_context.SaveChanges();

			return new RegistrationResult { Success = true };
		}

		private static bool VerifyPassword(string inputPassword, string storedPassword)
		{
			// Hash the input password
			byte[] inputPasswordHash = HashPassword(inputPassword);
			
			// Convert the stored password hash from base64 string to byte array
			byte[] storedPasswordHash = Convert.FromBase64String(storedPassword);

			// Compare the input password hash with the stored password hash
			return inputPasswordHash.SequenceEqual(storedPasswordHash);
		}

		private static byte[] HashPassword(string password)
		{
			// Create a new SHA256 hash object
			using (SHA256 sha256 = SHA256.Create())
			{
				// Convert the password to a byte array
				byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

				// Compute the hash of the password
				return sha256.ComputeHash(passwordBytes);
			}
		}

		public static string StorePassword(string password)
		{
			// Hash the password and convert it to a base64 string for storage
			byte[] hashedPasswordBytes = HashPassword(password);
			return Convert.ToBase64String(hashedPasswordBytes); // Return as string
		}
	}
}


