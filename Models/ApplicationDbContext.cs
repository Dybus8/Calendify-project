using Microsoft.EntityFrameworkCore;

namespace StarterKit.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> Users { get; set; }

        // ...existing code...
    }
}
