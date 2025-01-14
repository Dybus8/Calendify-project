import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './AdminDashboard.css';

interface Event {
  id: number;
  title: string;
  description: string;
  date: string;
  startTime: string;
  endTime: string;
  location: string;
}

interface EventFormData {
  title: string;
  description: string;
  date: string;
  startTime: string;
  endTime: string;
  location: string;
}

const AdminDashboard = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user || !user.isAdmin) {
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

  const handleCreateEvent = async (eventData: EventFormData) => {
    try {
      const response = await fetch('/api/events', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(eventData),
      });

      if (!response.ok) {
        throw new Error('Failed to create event');
      }

      const newEvent = await response.json();
      setEvents([...events, newEvent]);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('An unknown error occurred');
      }
    }
  };

  const handleDeleteEvent = async (eventId: number) => {
    try {
      const response = await fetch(`/api/events/${eventId}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error('Failed to delete event');
      }

      setEvents(events.filter(event => event.id !== eventId));
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
    <div className="admin-dashboard">
      <div className="top-buttons-admin-dashboard">
        <button onClick={() => navigate('/logout')}>Logout</button>
      </div>
      <h1>Admin Dashboard</h1>
      <h2>Events</h2>
      <div className="events-list">
        {events.map(event => (
          <div key={event.id} className="event-card">
            <h3>{event.title}</h3>
            <p>{event.description}</p>
            <p>Date: {event.date}</p>
            <p>Time: {event.startTime} - {event.endTime}</p>
            <p>Location: {event.location}</p>
            <button onClick={() => handleDeleteEvent(event.id)}>Delete Event</button>
          </div>
        ))}
      </div>
      <h2>Create New Event</h2>
      <form onSubmit={(e) => {
        e.preventDefault();
        const form = e.target as HTMLFormElement;
        const formData = new FormData(form);
        const eventData: EventFormData = {
          title: formData.get('title') as string,
          description: formData.get('description') as string,
          date: formData.get('date') as string,
          startTime: formData.get('startTime') as string,
          endTime: formData.get('endTime') as string,
          location: formData.get('location') as string,
        };
        handleCreateEvent(eventData);
      }}>
        <input type="text" name="title" placeholder="Title" required />
        <textarea name="description" placeholder="Description" required />
        <input type="date" name="date" required />
        <input type="time" name="startTime" required />
        <input type="time" name="endTime" required />
        <input type="text" name="location" placeholder="Location" required />
        <button type="submit">Create Event</button>
      </form>
    </div>
  );
};

export default AdminDashboard;
