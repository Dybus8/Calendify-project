using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using System;

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
            try 
            {
                return await _context.Events
                    .Include(e => e.Event_Attendances)
                        .ThenInclude(ea => ea.User)
                    .Select(e => new EventDTO
                    {
                        Id = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        Date = DateOnly.FromDateTime(e.EventDate),
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        Location = e.Location,
                        Reviews = _context.Reviews
                            .Where(r => r.EventId == e.EventId)
                            .Select(r => new ReviewDTO
                            {
                                Id = r.Id,
                                Comment = r.Comment,
                                Rating = r.Rating
                            }).ToList(),
                        Attendees = e.Event_Attendances.Select(ea => new AttendeeDTO
                        {
                            UserId = ea.User.UserId,
                            UserName = $"{ea.User.FirstName} {ea.User.LastName}"
                        }).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events");
                throw;
            }
        }

        public async Task<EventDTO> GetEventByIdAsync(int eventId)
        {
            try 
            {
                var @event = await _context.Events
                    .Include(e => e.Event_Attendances)
                        .ThenInclude(ea => ea.User)
                    .FirstOrDefaultAsync(e => e.EventId == eventId);

                return @event == null 
                    ? throw new EventNotFoundException(eventId)
                    : new EventDTO
                    {
                        Id = @event.EventId,
                        Title = @event.Title,
                        Description = @event.Description,
                        Date = DateOnly.FromDateTime(@event.EventDate),
                        StartTime = @event.StartTime,
                        EndTime = @event.EndTime,
                        Location = @event.Location,
                        Reviews = await _context.Reviews
                            .Where(r => r.EventId == eventId)
                            .Select(r => new ReviewDTO
                            {
                                Id = r.Id,
                                Comment = r.Comment,
                                Rating = r.Rating
                            }).ToListAsync(),
                        Attendees = @event.Event_Attendances.Select(ea => new AttendeeDTO
                        {
                            UserId = ea.User.UserId,
                            UserName = $"{ea.User.FirstName} {ea.User.LastName}"
                        }).ToList()
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving event with ID {eventId}");
                throw;
            }
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            try 
            {
                // Ensure only admins can create events
                if (!_loginService.IsAdmin())
                    throw new UserNotAuthorizedException("Only administrators can create events");

                var newEvent = new Event
                {
                    Title = eventCreateDTO.Title,
                    Description = eventCreateDTO.Description,
                    EventDate = eventCreateDTO.Date,
                    StartTime = eventCreateDTO.StartTime,
                    EndTime = eventCreateDTO.EndTime,
                    Location = eventCreateDTO.Location,
                    AdminApproval = true, // Assuming admin-created events are automatically approved
                    Event_Attendances = new List<Event_Attendance>()
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Event created: {newEvent.Title}");

                return await GetEventByIdAsync(newEvent.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                throw;
            }
        }

        public async Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO)
        {
            try 
            {
                // Ensure only admins can update events
                if (!_loginService.IsAdmin())
                    throw new UserNotAuthorizedException("Only administrators can update events");

                var existingEvent = await _context.Events.FindAsync(eventId)
                    ?? throw new EventNotFoundException(eventId);

                existingEvent.Title = eventUpdateDTO.Title;
                existingEvent.Description = eventUpdateDTO.Description;
                existingEvent.EventDate = eventUpdateDTO.Date;
                existingEvent.StartTime = eventUpdateDTO.StartTime;
                existingEvent.EndTime = eventUpdateDTO.EndTime;
                existingEvent.Location = eventUpdateDTO.Location;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Event updated: {existingEvent.Title}");

                return await GetEventByIdAsync(eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event {eventId}");
                throw;
            }
        }

        public async Task DeleteEventAsync(int eventId)
        {
            try 
            {
                // Ensure only admins can delete events
                if (!_loginService.IsAdmin())
                    throw new UserNotAuthorizedException("Only administrators can delete events");

                var @event = await _context.Events.FindAsync(eventId)
                    ?? throw new EventNotFoundException(eventId);

                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Event deleted: {eventId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event {eventId}");
                throw;
            }
        }

        // Other methods remain the same with similar error handling and logging...

        public async Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto)
        {
            try 
            {
                var user = _loginService.GetLoggedInUser()
                    ?? throw new UserNotAuthorizedException("User must be logged in to attend an event");

                var @event = await _context.Events.FindAsync(attendEventDto.EventId)
                    ?? throw new EventNotFoundException(attendEventDto.EventId);

                // Additional validations (same as before)
                if (@event.EventDate.Date < DateTime.Today.Date)
                    throw new EventAttendanceException("Cannot attend past events");

                var existingAttendance = await _context.EventAttendances
                    .FirstOrDefaultAsync(ea => 
                        ea.EventId == attendEventDto.EventId && 
                        ea.User.UserId == user.UserId);

                if (existingAttendance != null)
                    throw new EventAttendanceException("User is already attending this event");

                var eventAttendance = new Event_Attendance
                {
                    EventId = attendEventDto.EventId,
                    UserId = user.UserId,
                    User = user,
                    Event = @event,
                    Feedback = "",
                    Rating = 0 // Default rating
                };

                _context.EventAttendances.Add(eventAttendance);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User  {user.UserId} attended event {attendEventDto.EventId}");

                return await GetEventByIdAsync(attendEventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attending event");
                throw;
            }
        }

        public async Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId)
        {
            try 
            {
                _ = await _context.Events.FindAsync(eventId)
                    ?? throw new EventNotFoundException(eventId);

                return await _context.EventAttendances
                    .Where(ea => ea.EventId == eventId)
                    .Select(ea => new AttendeeDTO
                    {
                        UserId = ea.User.UserId,
                        UserName = $"{ea.User.FirstName} {ea.User.LastName}"
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving attendees for event {eventId}");
                throw;
            }
        }

        public async Task RemoveEventAttendanceAsync(int eventId, int userId)
        {
            try 
            {
                var @event = await _context.Events.FindAsync(eventId)
                    ?? throw new EventNotFoundException(eventId);

                var user = await _context.Users.FindAsync(userId)
                    ?? throw new UserNotAuthorizedException("User not found");

                var loggedInUser = _loginService.GetLoggedInUser();
                var isAdmin = _loginService.IsAdmin();

                if (loggedInUser?.UserId != userId && !isAdmin)
                    throw new UserNotAuthorizedException("You are not authorized to remove this attendance");

                var eventAttendance = await _context.EventAttendances
                    .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId)
                    ?? throw new EventAttendanceException("User is not attending this event");

                _context.EventAttendances.Remove(eventAttendance);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} removed from event {eventId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing attendance for user {userId} from event {eventId}");
                throw;
            }
        }
        public async Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO)
        {
            try 
            {
                // Validate event exists
                var @event = await _context.Events.FindAsync(eventId) 
                    ?? throw new EventNotFoundException(eventId);

                // Check if user is logged in
                var user = _loginService.GetLoggedInUser() 
                    ?? throw new UserNotAuthorizedException("User must be logged in to create a review");

                // Validate review
                if (reviewCreateDTO.Rating < 1 || reviewCreateDTO.Rating > 5)
                    throw new InvalidReviewException("Rating must be between 1 and 5");

                // Check if user has attended the event
                var hasAttended = await _context.EventAttendances
                    .AnyAsync(ea => ea.EventId == eventId && ea.User.UserId == user.UserId);

                if (!hasAttended)
                    throw new EventAttendanceException("Only event attendees can leave a review");

                var review = new Review
                {
                    EventId = eventId,
                    UserId = user.UserId,
                    Comment = reviewCreateDTO.Comment,
                    Rating = reviewCreateDTO.Rating,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Review created for event {eventId} by user {user.UserId}");

                return new ReviewDTO
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    Rating = review.Rating
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating review for event {eventId}");
                throw;
            }
        }
    }
}