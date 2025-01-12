using Microsoft.AspNetCore.Mvc;
using StarterKit.Models.DTOs;
using StarterKit.Services;
using StarterKit.Utils;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            ILoginService loginService, 
            ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto, [FromQuery] bool isAdminLogin = false)
        {
            try 
            {
                var status = _loginService.Login(loginDto.UserName, loginDto.Password, isAdminLogin);
                
                if (status == LoginStatus.Success)
                {
                    var user = _loginService.GetLoggedInUser();
                    return Ok(new 
                    { 
                        message = "Login successful",
                        isAdmin = _loginService.IsAdmin(),
                        userId = user.Id // Updated to use Id
                    });
                }
                else if (status == LoginStatus.IncorrectUsername)
                {
                    return BadRequest(new { message = "Email not found" });
                }
                else if (status == LoginStatus.IncorrectPassword)
                {
                    return BadRequest(new { message = "Incorrect password" });
                }
                else
                {
                    return BadRequest(new { message = "Login failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO registrationDto)
        {
            try
            {
                var user = await _loginService.RegisterUser(registrationDto);
                return CreatedAtAction(nameof(RegisterUser), new { id = user.Id }, 
                    new { 
                        message = "User registered successfully", 
                        userId = user.Id 
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            if (_loginService.IsLoggedIn())
            {
                var user = _loginService.GetLoggedInUser();
                return Ok(new 
                { 
                    isLoggedIn = true, 
                    isAdmin = _loginService.IsAdmin(),
                    name = user != null ? $"{user.FirstName} {user.LastName}" : null,
                    email = user?.Email
                });
            }

            return Ok(new 
            { 
                isLoggedIn = false, 
                isAdmin = false,
                name = (string)null, 
                email = (string)null 
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _loginService.Logout();
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpGet("log-users")]
        public async Task<IActionResult> LogUserAccounts()
        {
            var users = await _loginService.GetAllUsersAsync(); // Fetch all users
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.UserName}, Email: {user.Email}, Password: {user.Password}");
            }
            return Ok(new { message = "User accounts logged to console." });
        }

[HttpGet("test-hash")]
public IActionResult TestHash()
{
    string inputPassword = "password";
    string hashedPassword = EncryptionHelper.EncryptPassword(inputPassword);
    Console.WriteLine($"Input Password: {inputPassword}, Hashed Password: {hashedPassword}");
    return Ok(new { inputPassword, hashedPassword });
}


    }
}
