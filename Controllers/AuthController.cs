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
            var newUser = new UserAccount
            {
                UserName = userRegistrationDto.UserName,
                Password = EncryptionHelper.EncryptPassword(userRegistrationDto.Password),
                Email = userRegistrationDto.Email,

                FirstName = userRegistrationDto.FirstName,
                LastName = userRegistrationDto.LastName,
                RecuringDays = userRegistrationDto.RecuringDays,
                IsAdmin = false,
                Attendances = new List<Attendance>(),
                Event_Attendances = new List<Event_Attendance>()
            };

            await _userRepository.RegisterUserAsync(newUser);
            return Ok(new { message = "User registered successfully" });
        }
    }
}
