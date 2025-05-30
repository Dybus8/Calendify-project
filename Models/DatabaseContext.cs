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
            modelBuilder.Entity<UserAccount>()
                .HasIndex(p => p.UserName).IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasData(
                    new UserAccount { Id = 1, Email = "admin1@example.com", UserName = "admin1", Password = EncryptionHelper.EncryptPassword("password"), IsAdmin = true, FirstName = "Admin", LastName = "One", RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 2, Email = "admin2@example.com", UserName = "admin2", Password = EncryptionHelper.EncryptPassword("tooeasytooguess"), IsAdmin = true, FirstName = "Admin", LastName = "Two", RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 3, Email = "admin3@example.com", UserName = "admin3", Password = EncryptionHelper.EncryptPassword("helloworld"), IsAdmin = true, FirstName = "Admin", LastName = "Three", RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 4, Email = "admin4@example.com", UserName = "admin4", Password = EncryptionHelper.EncryptPassword("Welcome123"), IsAdmin = true, FirstName = "Admin", LastName = "Four", RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() },
                    new UserAccount { Id = 5, Email = "admin5@example.com", UserName = "admin5", Password = EncryptionHelper.EncryptPassword("Whatisapassword?"), IsAdmin = true, FirstName = "Admin", LastName = "Five", RecuringDays = "mo,tu,we,th,fr", Attendances = new List<Attendance>(), Event_Attendances = new List<Event_Attendance>() }
                );

            modelBuilder.Entity<Event>()
                .HasData(
                    new Event { EventId = 1, Title = "Event One", Description = "Description for Event One", EventDate = DateTime.Parse("2023-01-01"), StartTime = TimeSpan.Parse("10:00"), EndTime = TimeSpan.Parse("12:00"), Location = "Location One", AdminApproval = true, Event_Attendances = new List<Event_Attendance>() },
                    new Event { EventId = 2, Title = "Event Two", Description = "Description for Event Two", EventDate = DateTime.Parse("2023-02-01"), StartTime = TimeSpan.Parse("14:00"), EndTime = TimeSpan.Parse("16:00"), Location = "Location Two", AdminApproval = true, Event_Attendances = new List<Event_Attendance>() }
                );
        }
    }
}