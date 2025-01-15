import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext'; // Custom authentication context
import { useNavigate, useParams } from 'react-router-dom';
import './EventDetails.css';

interface Event {
  id: number;
  title: string;
  description: string;
  date: string;
  startTime: string;
  endTime: string;
  location: string;
  attendeesCount: number;
}

const EventDetails = () => {
  const { user } = useAuth(); // Get user data from context
  const navigate = useNavigate();
  const { eventId } = useParams<{ eventId: string }>();
  const [event, setEvent] = useState<Event | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [attending, setAttending] = useState(false); // Tracks attendance action state

  // Fetch event details
  useEffect(() => {
    if (!user) {
      navigate('/login');
      return;
    }

    if (!eventId) {
      setError('Event ID is missing.');
      setLoading(false);
      return;
    }

    const fetchEvent = async () => {
      try {
        const response = await fetch(`/api/events/${eventId}`);
        if (!response.ok) {
          throw new Error('Failed to fetch event details.');
        }
        const data = await response.json();
        setEvent(data);
      } catch (err) {
        setError((err as Error).message || 'An unknown error occurred.');
      } finally {
        setLoading(false);
      }
    };

    fetchEvent();
  }, [user, navigate, eventId]);

  // Handle attend event action
  const handleAttendEvent = async () => {
    if (!event) return;

    setAttending(true); // Start attendance action
    setError(null); // Clear previous errors

    try {
      const response = await fetch(`/api/events/${event.id}/attend`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user?.token}`, // Include auth token if required
        },
      });

      if (!response.ok) {
        if (response.status === 400) {
          throw new Error('You cannot attend this event.');
        }
        if (response.status === 403) {
          throw new Error('Unauthorized. Please log in again.');
        }
        if (response.status === 500) {
          throw new Error('Server error. Please try again later.');
        }
        throw new Error('Failed to attend event.');
      }

      const updatedEvent = await response.json();
      setEvent(updatedEvent); // Update event state with server response
    } catch (err) {
      setError((err as Error).message || 'An unknown error occurred.');
    } finally {
      setAttending(false); // End attendance action
    }
  };

  // Render UI
  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!event) {
    return <div>Event not found.</div>;
  }

  return (
    <div className="event-details-page">
      <div className="top-buttons-details">
        <button onClick={() => navigate('/dashboard')}>Back to Dashboard</button>
        <button onClick={() => navigate('/logout')}>Logout</button>
      </div>
      <h1>Event Details</h1>
      <div className="event-details-container">
        <div className="event-details">
          <h3>{event.title}</h3>
          <p>{event.description}</p>
          <p>Date: {event.date}</p>
          <p>Time: {event.startTime} - {event.endTime}</p>
          <p>Location: {event.location}</p>
          <p>Attendees: {event.attendeesCount}</p>
          <button onClick={handleAttendEvent} disabled={attending}>
            {attending ? 'Processing...' : 'Attend Event'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default EventDetails;