using StarterKit.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterKit.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetEventsAsync();
        Task<EventDTO> GetEventByIdAsync(int eventId);
        Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO);
        Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO);
        Task DeleteEventAsync(int eventId);
        Task<EventDTO> AttendEventAsync(int eventId, int userId);
        Task<bool> CheckEventAvailabilityAsync(int eventId);
        Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId);
    }
}
