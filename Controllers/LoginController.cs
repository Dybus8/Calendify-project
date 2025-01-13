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

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            var loginStatus = await _loginService.Login(loginDto.Username, loginDto.Password);
            return loginStatus switch
            {
                LoginStatus.Success => Ok(),
                LoginStatus.UserNotFound => NotFound("User not found"),
                LoginStatus.InvalidPassword => Unauthorized("Invalid password"),
                _ => StatusCode(500, "An error occurred during login")
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
        {
            if (registrationDto == null)
            {
                return BadRequest("Registration data is required");
            }

            try
            {
                var user = await _loginService.RegisterUser(registrationDto);
                return CreatedAtAction(nameof(GetUserInfo), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
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
