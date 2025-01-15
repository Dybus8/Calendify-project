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
  points: string;
  location: string;
}

interface EventFormData {
  title: string;
  description: string;
  date: string;
  startTime: string;
  endTime: string;
  points: string;
  location: string;
}

const AdminDashboard = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
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
  }, []);

  const handleCreateEvent = async (eventData: EventFormData) => {
    console.log("Creating event with data:", eventData); // Log the event data
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
    return <div className="loading-state">Loading<span>.</span><span>.</span><span>.</span></div>;
  }

  if (error) {
    return <div className="error-state">Error: {error}</div>;
  }

  return (
    <div className="admin-dashboard">
      <div className="dashboard-header">
        <h1>Admin Dashboard</h1>
        <button className="logout-button" onClick={() => navigate('/logout')}>Logout</button>
      </div>
      
      <div className="dashboard-content">
        <div className="events-section">
          <h2>Manage Events</h2>
          <div className="events-grid">
            {events.map(event => (
              <div key={event.id} className="event-card">
                <div className="event-header">
                  <h3>{event.title}</h3>
                  <span className="points-badge">{event.points} points</span>
                </div>
                <div className="event-body">
                  <p className="event-description">{event.description}</p>
                  <div className="event-details">
                    <p><i className="far fa-calendar"></i> {event.date}</p>
                    <p><i className="far fa-clock"></i> {event.startTime} - {event.endTime}</p>
                    <p><i className="fas fa-map-marker-alt"></i> {event.location}</p>
                  </div>
                </div>
                <div className="event-actions">
                  <button className="delete-button" onClick={() => handleDeleteEvent(event.id)}>
                    <i className="fas fa-trash"></i> Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="create-event-section">
          <h2>Create New Event</h2>
          <form className="event-form" onSubmit={(e) => {
            e.preventDefault();
            const form = e.target as HTMLFormElement;
            const formData = new FormData(form);
            const eventData: EventFormData = {
              title: formData.get('title') as string,
              description: formData.get('description') as string,
              date: formData.get('date') as string,
              points: formData.get('points') as string,
              startTime: formData.get('startTime') as string,
              endTime: formData.get('endTime') as string,
              location: formData.get('location') as string,
            };
            handleCreateEvent(eventData);
            form.reset();
          }}>
            <div className="form-group">
              <label htmlFor="title">Title</label>
              <input id="title" type="text" name="title" required />
            </div>
            <div className="form-group">
              <label htmlFor="description">Description</label>
              <textarea id="description" name="description" required />
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="date">Date</label>
                <input id="date" type="date" name="date" required />
              </div>
              <div className="form-group">
                <label htmlFor="points">Points</label>
                <input id="points" type="text" name="points" required />
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="startTime">Start Time</label>
                <input id="startTime" type="time" name="startTime" required />
              </div>
              <div className="form-group">
                <label htmlFor="endTime">End Time</label>
                <input id="endTime" type="time" name="endTime" required />
              </div>
            </div>
            <div className="form-group">
              <label htmlFor="location">Location</label>
              <input id="location" type="text" name="location" required />
            </div>
            <button type="submit" className="submit-button">Create Event</button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
