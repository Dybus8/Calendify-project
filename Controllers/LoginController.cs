using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;

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
        public IActionResult Login([FromBody] LoginModel model)
        {
            var result = _loginService.Login(model.Username, model.Password);
            if (result.Success)
            {
                return Ok(new { message = "Login successful", user = result.User });
            }
            return Unauthorized(new { message = result.Message });
        }

        [HttpGet("status")]
        public IActionResult GetLoginStatus()
        {
            var status = _loginService.GetLoginStatus();
            return Ok(new { isLoggedIn = status.IsLoggedIn, username = status.Username });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var result = _loginService.Register(new RegisterModel { /* ... */ });
            if (result.Success)
            {
                return Ok(new { message = "Registration successful" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}
