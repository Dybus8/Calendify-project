using Microsoft.AspNetCore.Mvc;
using StarterKit.Models.DTOs;
using StarterKit.Services;
using StarterKit.Utils;
using System.Threading.Tasks;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            if (loginDto == null)
            {
                _logger.LogWarning("Login attempt with null loginDto");
                return BadRequest("Login data is required");
            }

            _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);

            var result = await _loginService.LoginUserAsync(loginDto);
            if (result == null)
            {
                _logger.LogWarning("Invalid credentials for user: {Username}", loginDto.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User {Username} logged in successfully", loginDto.Username);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
        {
            if (registrationDto == null)
            {
                _logger.LogWarning("Registration attempt with null registrationDto");
                return BadRequest("Registration data is required");
            }

            try
            {
                var user = await _loginService.RegisterUser(registrationDto);
                return CreatedAtAction(nameof(GetUserInfo), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userSession = await _loginService.GetCurrentUserSessionInfoAsync();
                return Ok(userSession);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("User not logged in");
                return Unauthorized("User not logged in");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _loginService.Logout();
            return Ok();
        }

        [HttpGet("isloggedin")]
        public IActionResult IsLoggedIn()
        {
            return Ok(_loginService.IsLoggedIn());
        }

        [HttpGet("isadmin")]
        public IActionResult IsAdmin()
        {
            return Ok(_loginService.IsAdmin());
        }
    }
}
