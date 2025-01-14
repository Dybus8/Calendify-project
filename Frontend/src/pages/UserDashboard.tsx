import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './UserDashboard.css';

interface Event {
  id: number;
  title: string;
  description: string;
  date: string;
  startTime: string;
  endTime: string;
  location: string;
}

const UserDashboard = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      navigate('/login');
      return;
    }

    const fetchEvents = async () => {
      try {
        const response = await fetch('/api/events');
        if (!response.ok) {
          throw new Error('Failed to fetch events');
        }
        const data = await response.json();
        setEvents(data);
      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('An unknown error occurred');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, [user, navigate]);

  const handleAttendEvent = async (eventId: number) => {
    try {
      const response = await fetch(`/api/attendance/${eventId}/attend`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
      });
  
      if (!response.ok) {
        if (response.status === 405) {
          const allowHeader = response.headers.get('Allow');
          console.error('Allow header:', allowHeader);
        }
        const errorText = await response.text();
        console.error('Error response:', errorText);
        throw new Error('Failed to attend event');
      }
  
      const updatedEvent = await response.json();
      setEvents(events.map(event => (event.id === updatedEvent.id ? updatedEvent : event)));
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('An unknown error occurred');
      }
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div className="user-dashboard">
      <h1>User Dashboard</h1>
      <h2>Events</h2>
      <div className="event-card-container">
        {events.map(event => (
          <div key={event.id} className="event-card">
            <h3>{event.title}</h3>
            <p>{event.description}</p>
            <p>Date: {event.date}</p>
            <p>Time: {event.startTime} - {event.endTime}</p>
            <p>Location: {event.location}</p>
            <button onClick={() => navigate(`/event_details/${event.id}`)}>View Details</button>
            <button onClick={() => handleAttendEvent(event.id)}>Attend Event</button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default UserDashboard;
