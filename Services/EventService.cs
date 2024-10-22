using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StarterKit.Services
{
    public class EventService : IEventService
    {
        private readonly DatabaseContext _context;

        public EventService(DatabaseContext context)
        {
            _context = context;
        }

        public List<Event> GetEvents()
        {
            return _context.Event.Include(e => e.Reviews).Include(e => e.Attendees).ToList();
        }

        public EventResult CreateEvent(EventModel model)
        {
            var eventEntity = new Event
            {
                Title = model.Title,
                Description = model.Description,
                EventDate = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location
            };

            _context.Event.Add(eventEntity);
            _context.SaveChanges();

            return new EventResult { Success = true };
        }

        public EventResult UpdateEvent(int eventId, EventModel model)
        {
            var eventEntity = _context.Event.Find(eventId);
            if (eventEntity == null)
            {
                return new EventResult { Success = false, Message = "Event not found" };
            }

            eventEntity.Title = model.Title;
            eventEntity.Description = model.Description;
            eventEntity.EventDate = model.Date;
            eventEntity.StartTime = model.StartTime;
            eventEntity.EndTime = model.EndTime;
            eventEntity.Location = model.Location;

            _context.SaveChanges();

            return new EventResult { Success = true };
        }

        public EventResult DeleteEvent(int eventId)
        {
            var eventEntity = _context.Event.Find(eventId);
            if (eventEntity == null)
            {
                return new EventResult { Success = false, Message = "Event not found" };
            }

            _context.Event.Remove(eventEntity);
            _context.SaveChanges();

            return new EventResult { Success = true };
        }
    }
}