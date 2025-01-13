using StarterKit.Models;
using System.Threading.Tasks;

namespace StarterKit.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(UserAccount newUser);
        Task<List<UserAccount>> GetAllUsersAsync();
        object GetUserByIdAsync(int value);
        Task<UserAccount> GetUserByUsernameAsync(string username);
        // Other user-related methods...
    }
}