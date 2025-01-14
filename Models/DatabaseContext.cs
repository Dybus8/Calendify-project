using Microsoft.EntityFrameworkCore;
using StarterKit.Utils;
using StarterKit.Models;
using System.Collections.Generic;

namespace StarterKit.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<UserAccount> UserAccounts { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Event_Attendance> EventAttendances { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints here
            modelBuilder.Entity<UserAccount>().Property(u => u.Points).HasDefaultValue(0);
            modelBuilder.Entity<Event_Attendance>().Property(ea => ea.Points).HasDefaultValue(0);
            modelBuilder.Entity<Event>().Property(e => e.Points).HasDefaultValue(0);

            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .IsRequired()
                .HasDefaultValue(0)
                .HasAnnotation("MinValue", 1)
                .HasAnnotation("MaxValue", 10);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserAccount>()
                .HasIndex(p => p.UserName).IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasData(
                    new UserAccount { Id = 1, Email = "admin1@example.com", UserName = "admin1", Password = EncryptionHelper.EncryptPassword("password"), IsAdmin = true, FirstName = "Admin", LastName = "One", Points = 0, RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 2, Email = "admin2@example.com", UserName = "admin2", Password = EncryptionHelper.EncryptPassword("tooeasytooguess"), IsAdmin = true, FirstName = "Admin", LastName = "Two", Points = 0, RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 3, Email = "admin3@example.com", UserName = "admin3", Password = EncryptionHelper.EncryptPassword("helloworld"), IsAdmin = true, FirstName = "Admin", LastName = "Three", Points = 0, RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 4, Email = "admin4@example.com", UserName = "admin4", Password = EncryptionHelper.EncryptPassword("Welcome123"), IsAdmin = true, FirstName = "Admin", LastName = "Four", Points = 0, RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 5, Email = "admin5@example.com", UserName = "admin5", Password = EncryptionHelper.EncryptPassword("Whatisapassword?"), IsAdmin = true, FirstName = "Admin", LastName = "Five", Points = 0, RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() }
                );

            modelBuilder.Entity<Event>()
                .HasData(
                    new Event { EventId = 1, Title = "Event One", Description = "Description for Event One", EventDate = DateTime.Parse("2023-01-01"), StartTime = TimeSpan.Parse("10:00"), EndTime = TimeSpan.Parse("12:00"), Location = "Location One", Points = 0, AdminApproval = true, Event_Attendances = new List<Event_Attendance>() },
                    new Event { EventId = 2, Title = "Event Two", Description = "Description for Event Two", EventDate = DateTime.Parse("2023-02-01"), StartTime = TimeSpan.Parse("14:00"), EndTime = TimeSpan.Parse("16:00"), Location = "Location Two", Points = 0, AdminApproval = true, Event_Attendances = new List<Event_Attendance>() }
                );
        }
    }
}