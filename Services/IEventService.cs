using StarterKit.Models;

namespace StarterKit.Services
{
    public interface IEventService
    {
        List<Event> GetEvents();
        EventResult CreateEvent(EventModel model);
        EventResult UpdateEvent(int eventId, EventModel model);
        EventResult DeleteEvent(int eventId);

    }
}