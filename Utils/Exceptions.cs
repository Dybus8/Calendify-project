using System;

namespace StarterKit.Utils
{
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException(int eventId) 
            : base($"Event with ID {eventId} not found") 
        { 
            EventId = eventId;
        }

        public int EventId { get; }
    }

    public class UserNotAuthorizedException : Exception
    {
        public UserNotAuthorizedException(string message) : base(message) { }
    }

    public class InvalidReviewException : Exception
    {
        public InvalidReviewException(string message) : base(message) { }
    }

    public class EventAttendanceException : Exception
    {
        public EventAttendanceException(string message) : base(message) { }
    }
}