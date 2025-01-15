using Microsoft.EntityFrameworkCore;
using StarterKit.Models;

namespace StarterKit.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Event_Attendance> Event_Attendances { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Review> Reviews { get; set; } // New property added

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and other settings
        }
    }
}
