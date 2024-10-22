using StarterKit.Models;

namespace StarterKit.Services
{
	public interface ILoginService
	{
		LoginResult Login(string username, string password);
		LoginStatus GetLoginStatus();
		RegistrationResult Register(StarterKit.Models.RegisterModel model);
	}
}