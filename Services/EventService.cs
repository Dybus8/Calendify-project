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
        private readonly ILogger<EventService> _logger;

        public EventService(DatabaseContext context, ILogger<EventService> logger)
        {
            _context = context;
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
                    EventDate = DateOnly.FromDateTime(e.EventDate),
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location,
                    AttendeesCount = e.EventAttendances.Count
                }).ToListAsync();
        }

        public async Task<EventDTO> GetEventByIdAsync(int eventId)
        {
            var @event = await _context.Events
                .Include(e => e.EventAttendances)
                .FirstOrDefaultAsync(e => e.EventId == eventId)
                ?? throw new EventNotFoundException(eventId);

            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                EventDate = DateOnly.FromDateTime(@event.EventDate),
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location,
                AttendeesCount = @event.EventAttendances.Count
            };
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            var newEvent = new Event
            {
                Title = eventCreateDTO.Title,
                Description = eventCreateDTO.Description,
                EventDate = eventCreateDTO.EventDate,
                StartTime = eventCreateDTO.StartTime,
                EndTime = eventCreateDTO.EndTime,
                Location = eventCreateDTO.Location,
                EventAttendances = new List<Event_Attendance>()
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = newEvent.EventId,
                Title = newEvent.Title,
                Description = newEvent.Description,
                EventDate = DateOnly.FromDateTime(newEvent.EventDate),
                StartTime = newEvent.StartTime,
                EndTime = newEvent.EndTime,
                Location = newEvent.Location,
                AttendeesCount = 0
            };
        }

        public async Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO)
        {
            var @event = await _context.Events.FindAsync(eventId)
                ?? throw new EventNotFoundException(eventId);

            @event.Title = eventUpdateDTO.Title;
            @event.Description = eventUpdateDTO.Description;
            @event.EventDate = eventUpdateDTO.EventDate;
            @event.StartTime = eventUpdateDTO.StartTime;
            @event.EndTime = eventUpdateDTO.EndTime;
            @event.Location = eventUpdateDTO.Location;

            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                EventDate = DateOnly.FromDateTime(@event.EventDate),
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location,
                AttendeesCount = @event.EventAttendances.Count
            };
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId)
                ?? throw new EventNotFoundException(eventId);

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        public async Task<EventDTO> AttendEventAsync(int eventId, int userId)
        {
            var attendEventDto = new AttendEventDTO { EventId = eventId, UserId = userId };
            return await AttendEventAsync(attendEventDto);
        }

        public async Task<bool> CheckEventAvailabilityAsync(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
            {
                throw new EventNotFoundException(eventId);
            }

            return @event.EventAttendances.Count < 100; // Assuming 100 is the capacity limit
        }

        // Other methods...
        public async Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto)
        {
            var @event = await _context.Events
                .Include(e => e.EventAttendances)
                .FirstOrDefaultAsync(e => e.EventId == attendEventDto.EventId)
                ?? throw new EventNotFoundException(attendEventDto.EventId);

            if (@event.EventDate.Date < DateTime.Today.Date)
                throw new EventAttendanceException("Cannot attend past events.");

            if (@event.EventAttendances.Count >= 100)
                throw new EventAttendanceException("Event is at full capacity.");

            var existingAttendance = @event.EventAttendances
                .FirstOrDefault(ea => ea.UserAccount.Id == attendEventDto.UserId);

            if (existingAttendance != null)
                throw new EventAttendanceException("User is already attending this event.");

            var userAccount = await _context.UserAccounts.FindAsync(attendEventDto.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            var eventAttendance = new Event_Attendance
            {
                EventId = attendEventDto.EventId,
                UserAccount = userAccount,
                Feedback = "",
                Rating = 0,
                Event = @event // Set the Event property
            };

            @event.EventAttendances.Add(eventAttendance);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {attendEventDto.UserId} attended event {attendEventDto.EventId}");

            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                EventDate = DateOnly.FromDateTime(@event.EventDate),
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location,
                Attendees = @event.EventAttendances
                    .Select(ea => new AttendeeDTO
                    {
                        Id = ea.UserAccount.Id,
                        UserName = $"{ea.UserAccount.FirstName} {ea.UserAccount.LastName}"
                    }).ToList()
            };
        }

        public async Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId)
        {
            var @event = await _context.Events
                .Include(e => e.EventAttendances)
                .ThenInclude(ea => ea.UserAccount)
                .FirstOrDefaultAsync(e => e.EventId == eventId)
                ?? throw new EventNotFoundException(eventId);

            return @event.EventAttendances.Select(ea => new AttendeeDTO
            {
                Id = ea.UserAccount.Id,
                UserName = $"{ea.UserAccount.FirstName} {ea.UserAccount.LastName}"
            }).ToList();
        }

        public async Task RemoveEventAttendanceAsync(int eventId, int Id)
        {
            var @event = await _context.Events
                .Include(e => e.EventAttendances)
                .FirstOrDefaultAsync(e => e.EventId == eventId)
                ?? throw new EventNotFoundException(eventId);

            var attendance = @event.EventAttendances.FirstOrDefault(ea => ea.Event_AttendanceId == Id);
            if (attendance != null)
            {
                _context.EventAttendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO)
        {
            // Implementation for creating a review for an event
            throw new NotImplementedException();
        }

        public async Task<bool> CheckEventAvailabilityAsync(int eventId, AttendEventDTO attendEventDto)
        {
            // Implementation for checking event availability
            throw new NotImplementedException();
        }

        public async Task ManageOfficeAttendanceAsync(OfficeAttendanceDTO officeAttendanceDto)
        {
            // Implementation for managing office attendance
            throw new NotImplementedException();
        }
    }
}
