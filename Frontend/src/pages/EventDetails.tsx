import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
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
  const { user } = useAuth();
  const navigate = useNavigate();
  const { eventId } = useParams<{ eventId: string }>(); // Ensure eventId is extracted
  const [event, setEvent] = useState<Event | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      navigate('/login');
      return;
    }

    if (!eventId) {
      setError('Event ID is missing');
      setLoading(false);
      return;
    }

    const fetchEvent = async () => {
      try {
        console.log(`Fetching event details for event ID: ${eventId}`); // Log event ID
        const response = await fetch(`/api/events/${eventId}`);
        if (!response.ok) {
          throw new Error(response.statusText);
        }
        const data = await response.json();
        console.log('Event details fetched:', data); // Log fetched event details
        setEvent(data);
      } catch (err) {
        console.error('Error fetching event details:', err); // Log error
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('An unknown error occurred');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchEvent();
  }, [user, navigate, eventId]);

  const handleAttendEvent = async () => {
    if (!event) return;

    // Check if the event is in the past
    const eventDate = new Date(event.date);
    if (eventDate < new Date()) {
      setError('Cannot attend past events.');
      return;
    }

    // Check if the event is at capacity
    if (event.attendeesCount >= 100) {
      setError('This event is at capacity.');
      return;
    }

    navigate(`/events/${event.id}/attend`);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!event) {
    return <div>Event not found</div>;
  }

  return (
    <div className="event-details-page">
      <div className="top-buttons-details">
        <button onClick={() => navigate('/dashboard')}>Back to Dashboard</button>
        <button onClick={() => navigate('/logout')}>Logout</button>
      </div>
      <h1>Event details</h1>
      <div className="event-details-container">
        <div className="event-details">
          <h3>{event.title}</h3>
          <p>{event.description}</p>
          <p>Date: {event.date}</p>
          <p>Time: {event.startTime} - {event.endTime}</p>
          <p>Location: {event.location}</p>
          <button onClick={() => navigate ("/events/${eventId}/attend")}>Attend Event</button>
        </div>
      </div>
    </div>
  );
};

export default EventDetails;
