using StarterKit.Models;
using StarterKit.Models.DTOs;

namespace StarterKit.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetEventsAsync();
        Task<EventDTO> GetEventByIdAsync(int eventId);
        Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO);
        Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO);
        Task DeleteEventAsync(int eventId);
        Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO);
        Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto);
        Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId);
        Task RemoveEventAttendanceAsync(int eventId, int userId);
    }
}