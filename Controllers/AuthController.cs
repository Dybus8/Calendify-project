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
				IsAdmin = false,
				Attendances = new List<Attendance>(),
				Points = 0,
				Event_Attendances = new List<Event_Attendance>()
			};

			await _userRepository.RegisterUserAsync(newUser);
			return Ok(new { message = "User registered successfully" });
		}
	}
}
