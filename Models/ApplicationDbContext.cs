using Microsoft.EntityFrameworkCore;

namespace StarterKit.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
