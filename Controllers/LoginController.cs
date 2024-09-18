using Microsoft.AspNetCore.Mvc;

namespace StarterKit.Controllers
{
    [Route("api/v1/Login")]
    public class LoginController : Controller
    {
        private static bool _isAdminLoggedIn = false; // Simulated login status

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginBody loginBody)
        {
            if (loginBody == null || string.IsNullOrEmpty(loginBody.Username) || string.IsNullOrEmpty(loginBody.Password))
            {
                return BadRequest("Username and password are required.");
            }

            if (loginBody.Username == "admin" && loginBody.Password == "password123")
            {
                _isAdminLoggedIn = true;
                return Ok("Login successful");
            }

            return Unauthorized("Incorrect username or password");
        }

        [HttpGet("IsAdminLoggedIn")]
        public IActionResult IsAdminLoggedIn()
        {
            if (_isAdminLoggedIn)
            {
                return Ok("Admin is logged in");
            }

            return Unauthorized("Admin is not logged in");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            _isAdminLoggedIn = false;
            return Ok("Logged out");
        }
    }

    public class LoginBody
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
