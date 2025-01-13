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
            await _context.UserAccounts.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAccount>> GetAllUsersAsync()
        {
            return await _context.UserAccounts.ToListAsync();
        }


        public async Task<UserAccount> GetUserByIdAsync(int value)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Id == value);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id '{value}' not found.");
            }
            return user;
        }

        public async Task<UserAccount> GetUserByUsernameAsync(string username)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username '{username}' not found.");
            }
            return user;
        }

        public async Task RegisterUserAsync(UserAccount newUser)
        {
            await _context.UserAccounts.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        Task<UserAccount> IUserRepository.GetUserByIdAsync(int value)
        {
            return GetUserByIdAsync(value);
        }
    }
}
