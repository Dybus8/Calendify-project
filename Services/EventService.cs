using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
						.ThenInclude(ea => ea.UserAccount)
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
							Id = ea.UserAccount.Id,
							UserName = $"{ea.UserAccount.FirstName} {ea.UserAccount.LastName}"
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
						.ThenInclude(ea => ea.UserAccount)
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
							Id = ea.UserAccount.Id,
							UserName = $"{ea.UserAccount.FirstName} {ea.UserAccount.LastName}"
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
					Points = eventCreateDTO.Points,
					AdminApproval = true,
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

		public async Task<EventDTO> AttendEventAsync(int eventId, int userId)
		{
			try 
			{
				var @event = await _context.Events.FindAsync(eventId)
					?? throw new EventNotFoundException(eventId);

				if (@event.EventDate.Date < DateTime.Today.Date)
					throw new EventAttendanceException("Cannot attend past events");

				var existingAttendance = await _context.EventAttendances
					.FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserAccount.Id == userId);

				if (existingAttendance != null)
					throw new EventAttendanceException("User is already attending this event");

				var userAccount = await _context.UserAccounts.FindAsync(userId)
					?? throw new KeyNotFoundException("User not found");

				var eventAttendance = new Event_Attendance
				{
					EventId = eventId,
					Id = userId,
					UserAccount = userAccount,
					Event = @event,
					Points = 0,
					Feedback = "",
					Rating = 0
				};

				_context.EventAttendances.Add(eventAttendance);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"User {userId} attended event {eventId}");

				return await GetEventByIdAsync(eventId);
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
						Id = ea.UserAccount.Id,
						UserName = $"{ea.UserAccount.FirstName} {ea.UserAccount.LastName}"
					}).ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error retrieving attendees for event {eventId}");
				throw;
			}
		}

		public async Task RemoveEventAttendanceAsync(int eventId, int Id)
		{
			try 
			{
				var @event = await _context.Events.FindAsync(eventId)
					?? throw new EventNotFoundException(eventId);

				var user = await _context.UserAccounts.FindAsync(Id)
					?? throw new UserNotAuthorizedException("User not found");

				var loggedInUser = await _loginService.GetCurrentUserSessionInfoAsync();
				var isAdmin = _loginService.IsAdmin();

				if (loggedInUser?.UserId != Id && !isAdmin)
					throw new UserNotAuthorizedException("You are not authorized to remove this attendance");

				var eventAttendance = await _context.EventAttendances
					.FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.Id == Id)
					?? throw new EventAttendanceException("User is not attending this event");

				_context.EventAttendances.Remove(eventAttendance);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"User {Id} removed from event {eventId}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error removing attendance for user {Id} from event {eventId}");
				throw;
			}
		}

		public async Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO)
		{
			try 
			{
				var @event = await _context.Events.FindAsync(eventId) 
					?? throw new EventNotFoundException(eventId);

				var userSession = await _loginService.GetCurrentUserSessionInfoAsync() 
					?? throw new UserNotAuthorizedException("User must be logged in to create a review");

				if (reviewCreateDTO.Rating < 1 || reviewCreateDTO.Rating > 5)
					throw new InvalidReviewException("Rating must be between 1 and 5");

				var hasAttended = await _context.EventAttendances
					.AnyAsync(ea => ea.EventId == eventId && ea.UserAccount.Id == userSession.UserId);

				if (!hasAttended)
					throw new EventAttendanceException("Only event attendees can leave a review");

				var review = new Review
				{
					EventId = eventId,
					Id = userSession.UserId,
					Comment = reviewCreateDTO.Comment,
					Rating = reviewCreateDTO.Rating,
					CreatedDate = DateTime.UtcNow
				};

				_context.Reviews.Add(review);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"Review created for event {eventId} by user {userSession.UserId}");

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

		public async Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto)
		{
			try
			{
				var @event = await _context.Events.FindAsync(attendEventDto.EventId)
					?? throw new EventNotFoundException(attendEventDto.EventId);

				if (@event.EventDate.Date < DateTime.Today.Date)
					throw new EventAttendanceException("Cannot attend past events");

				var existingAttendance = await _context.EventAttendances
					.FirstOrDefaultAsync(ea => ea.EventId == attendEventDto.EventId && ea.UserAccount.Id == attendEventDto.UserId);

				if (existingAttendance != null)
					throw new EventAttendanceException("User is already attending this event");

				var userAccount = await _context.UserAccounts.FindAsync(attendEventDto.UserId)
					?? throw new KeyNotFoundException("User not found");

				var eventAttendance = new Event_Attendance
				{
					EventId = attendEventDto.EventId,
					Id = attendEventDto.UserId,
					UserAccount = userAccount,
					Event = @event,
					Points = 0,
					Feedback = "",
					Rating = 0
				};

				_context.EventAttendances.Add(eventAttendance);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"User {attendEventDto.UserId} attended event {attendEventDto.EventId}");

				return await GetEventByIdAsync(attendEventDto.EventId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error attending event");
				throw;
			}
		}

		public async Task<bool> CheckEventAvailabilityAsync(int eventId, AttendEventDTO attendEventDto)
		{
			try
			{
				var @event = await _context.Events.FindAsync(eventId)
					?? throw new EventNotFoundException(eventId);

				// Check if the event date is in the future
				if (@event.EventDate.Date < DateTime.Today.Date)
					return false;

				// Check if the event is at capacity (if applicable)
				var attendeeCount = await _context.EventAttendances
					.CountAsync(ea => ea.EventId == eventId);

				// Assuming a max capacity of 100 for this example
				if (attendeeCount >= 100)
					return false;

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error checking availability for event {eventId}");
				throw;
			}
		}

		public async Task ManageOfficeAttendanceAsync(OfficeAttendanceDTO officeAttendanceDto)
		{
			try
			{
				var user = await _context.UserAccounts.FindAsync(officeAttendanceDto.UserId)
					?? throw new KeyNotFoundException("User not found");

				// Check if the user already has an attendance record for the given date
				var existingAttendance = await _context.Attendances
					.FirstOrDefaultAsync(a => 
						a.User.Id == officeAttendanceDto.UserId && 
						a.AttendanceDate.Date == officeAttendanceDto.AttendanceDate.Date);

				if (existingAttendance != null)
				{
					// Update existing attendance
					existingAttendance.Notes = officeAttendanceDto.Notes;
				}
				else
				{
					// Create new attendance
					var attendance = new Attendance
					{
						User = user,
						AttendanceDate = officeAttendanceDto.AttendanceDate,
						Notes = officeAttendanceDto.Notes
					};

					_context.Attendances.Add(attendance);
				}

				await _context.SaveChangesAsync();
				_logger.LogInformation($"Office attendance recorded for user {officeAttendanceDto.UserId} on {officeAttendanceDto.AttendanceDate}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error managing office attendance");
				throw;
			}
		}
	}
}
