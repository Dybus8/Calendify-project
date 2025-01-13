using StarterKit.Models;
using System.Threading.Tasks;

namespace StarterKit.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(UserAccount newUser);
        Task<List<UserAccount>> GetAllUsersAsync();
        Task<UserAccount> GetUserByIdAsync(int value);
        Task<UserAccount> GetUserByUsernameAsync(string username);
        Task RegisterUserAsync(UserAccount newUser);
        // Other user-related methods...
    }
}