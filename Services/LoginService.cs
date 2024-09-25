using Microsoft.AspNetCore.Http.HttpResults;
using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }

public enum ADMIN_SESSION_KEY { adminLoggedIn }

public class LoginService : ILoginService
{

	private readonly DatabaseContext _context;

	public LoginService(DatabaseContext context)
	{
		_context = context;
	}

	public LoginStatus CheckPassword(string username, string inputPassword)
	{
		// TODO: Make this method check the password with what is in the database
		foreach (var i in _context.Admin)
		{
			if (i.UserName == username)
			{
				// Hash the entered password
				string enteredPasswordHash = EncryptionHelper.HashPassword(inputPassword);
				
				// Compare the hashed entered password with the stored hash
				if (enteredPasswordHash == i.Password){
					return LoginStatus.Success;
				}
				return LoginStatus.IncorrectPassword;
				
			}
		}
		return LoginStatus.IncorrectUsername;
	}
}