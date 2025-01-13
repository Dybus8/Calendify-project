using StarterKit.Models;
using StarterKit.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StarterKit.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(UserAccount newUser)
        {
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAccount>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<UserAccount> GetUserByIdAsync(int value)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == value);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id '{value}' not found.");
            }
            return user;
        }

        public async Task<UserAccount> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username '{username}' not found.");
            }
            return user;
        }

        object IUserRepository.GetUserByIdAsync(int value)
        {
            throw new NotImplementedException();
        }
    }
}
