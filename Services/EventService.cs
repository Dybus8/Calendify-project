using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StarterKit.Services
{
    public class EventService : IEventService
    {
        private readonly DatabaseContext _context;
        private readonly ILoginService _loginService;
        private readonly ILogger<EventService> _logger;

        public EventService(
            DatabaseContext context, 
            ILoginService loginService,
            ILogger<EventService> logger)
        {
            _context = context;
            _loginService = loginService;
            _logger = logger;
        }

        public async Task<IEnumerable<EventDTO>> GetEventsAsync()
        {
            return await _context.Events
                .Select(e => new EventDTO
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Location = e.Location,
                    // Other properties...
                })
                .ToListAsync();
        }

        public async Task<EventDTO> GetEventByIdAsync(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null) throw new EventNotFoundException(eventId);
            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                Location = @event.Location,
                // Other properties...
            };
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            try
            {
                // Parse the date and time strings
                DateTime eventDate = DateTime.Parse(eventCreateDTO.Date);
                TimeSpan startTime = TimeSpan.Parse(eventCreateDTO.StartTime);
                TimeSpan endTime = TimeSpan.Parse(eventCreateDTO.EndTime);

                var newEvent = new Event
                {
                    Title = eventCreateDTO.Title,
                    Description = eventCreateDTO.Description,
                    Location = eventCreateDTO.Location,
                    EventDate = eventDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Points = eventCreateDTO.Points,
                    Event_Attendances = new List<Event_Attendance>()
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                return await GetEventByIdAsync(newEvent.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event with data: {@EventData}", eventCreateDTO);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null) throw new EventNotFoundException(eventId);
            existingEvent.Title = eventUpdateDTO.Title;
            existingEvent.Description = eventUpdateDTO.Description;
            existingEvent.Location = eventUpdateDTO.Location;
            // Other properties...
            await _context.SaveChangesAsync();
            return await GetEventByIdAsync(eventId);
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null) throw new EventNotFoundException(eventId);
            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<UserAccountDTO> GetUserAccountAsync(int userId)
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount == null) throw new KeyNotFoundException("User not found");
            return new UserAccountDTO
            {
                UserId = userAccount.Id,
                Username = userAccount.UserName,
                Email = userAccount.Email,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                Points = userAccount.Points
            };
        }

        public async Task<UserAccountDTO> UpdateUserAccountAsync(int userId, UserAccountDTO userAccountDTO)
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount == null) throw new KeyNotFoundException("User not found");
            userAccount.FirstName = userAccountDTO.FirstName;
            userAccount.LastName = userAccountDTO.LastName;
            userAccount.Email = userAccountDTO.Email;
            await _context.SaveChangesAsync();
            return userAccountDTO;
        }

        public async Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto)
        {
            var attendance = new Event_Attendance
            {
                EventId = attendEventDto.EventId,
                UserId = attendEventDto.UserId,
                AttendanceDate = DateTime.UtcNow,
                Feedback = "", // Set default or handle as needed
                Points = 0 // Set default or handle as needed
            };

            _context.Event_Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(attendEventDto.EventId);
        }

        public async Task<bool> CheckEventAvailabilityAsync(int eventId, AttendEventDTO attendEventDto)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null) return false;

            // No longer checking MaxAttendees
            var currentAttendanceCount = await _context.Event_Attendances.CountAsync(a => a.EventId == eventId);
            return currentAttendanceCount < 100; // Assuming a default capacity
        }

        public async Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId)
        {
            return await _context.Event_Attendances
                .Where(a => a.EventId == eventId)
                .Select(a => new AttendeeDTO
                {
                    Id = a.UserId,
                    UserName = a.UserAccount.UserName // Updated to use UserAccount
                })
                .ToListAsync();
        }

        public async Task RemoveEventAttendanceAsync(int eventId, int Id)
        {
            var attendance = await _context.Event_Attendances
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == Id);
            if (attendance != null)
            {
                _context.Event_Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ReviewDTO> CreateReviewAsync(int eventId, StarterKit.Models.DTOs.ReviewCreateDTO reviewCreateDTO)
        {
            var review = new Review
            {
                EventId = eventId,
                Rating = reviewCreateDTO.Rating,
                Comment = reviewCreateDTO.Comment,
                CreatedDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                EventId = eventId, // Added EventId
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate
            };
        }

        public async Task ManageOfficeAttendanceAsync(OfficeAttendanceDTO officeAttendanceDto)
        {
            // Implementation for managing office attendance
        }
    }
}
