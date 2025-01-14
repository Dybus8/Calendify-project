using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarterKit.Models;

namespace CalendifyProject.Repositories
{
    public interface IEventRepository
    {
        Task<Event> GetEventByIdAsync(Guid eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task AddEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updatedEvent);
        Task DeleteEventAsync(Guid eventId);
        Task<Event> GetEventByIdAsync(int eventId);
    }
}