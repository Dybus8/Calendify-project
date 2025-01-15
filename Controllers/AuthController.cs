using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Repositories;
using StarterKit.Utils;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userRegistrationDto)
        {
            if (await _userRepository.UserNameExistsAsync(userRegistrationDto.UserName))
            {
                return Conflict(new { message = "Username already exists" });
            }

            var newUser = new UserAccount
            {
                UserName = userRegistrationDto.UserName,
                Password = EncryptionHelper.EncryptPassword(userRegistrationDto.Password ?? throw new ArgumentNullException(nameof(userRegistrationDto.Password))),
                Email = userRegistrationDto.Email ?? throw new ArgumentNullException(nameof(userRegistrationDto.Email)),
                FirstName = userRegistrationDto.FirstName ?? throw new ArgumentNullException(nameof(userRegistrationDto.FirstName)),
                LastName = userRegistrationDto.LastName ?? throw new ArgumentNullException(nameof(userRegistrationDto.LastName)),
                RecuringDays = userRegistrationDto.RecuringDays ?? "",
                Points = userRegistrationDto.Points,
                IsAdmin = false,
                Attendances = new List<Attendance>(),
                Event_Attendances = new List<Event_Attendance>()
            };

            await _userRepository.RegisterUserAsync(newUser);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserIdFromToken(); // Replace with actual logic to get user ID
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userDto = new UserAccountDTO
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email, // Include Email
                FirstName = user.FirstName,
                LastName = user.LastName,
                Points = user.Points
            };

            return Ok(userDto);
        }

        private int GetUserIdFromToken()
        {
            // Implement logic to extract user ID from the token or session
            // This is a placeholder and should be replaced with actual implementation
            return 1; // Example user ID
        }
    }
}
