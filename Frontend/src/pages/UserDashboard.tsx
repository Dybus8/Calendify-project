import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './UserDashboard.css';

interface Event {
  id: number;
  title: string;
  description: string;
  location: string;
  eventDate: string;
  startTime: string;
  endTime: string;
  points: number;
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
      console.log('Fetching events for user dashboard...');
      try {
        const response = await axios.get('/api/events');
        console.log('Events fetched successfully:', response.data);
        setEvents(response.data);
      } catch (error) {
        console.error('Error fetching events:', error);
        if (axios.isAxiosError(error)) {
          console.error('Response data:', error.response?.data);
          console.error('Response status:', error.response?.status);
        }
      } finally {
        setLoading(false);
      }
    };

    console.log('UserDashboard mounted, user:', user);
    fetchEvents();
  }, [user, navigate]);

  const handleAttendEvent = async (eventId: number) => {
    try {
      const response = await fetch(`/api/events/${eventId}/attend`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ eventId }),
      });

      if (!response.ok) {
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
      <div className="top-buttons-user-dashboard">
        <button onClick={() => navigate('/logout')}>Logout</button>
      </div>
      <h1>User Dashboard</h1>
      <h2>Events</h2>
      <div className="event-card-container">
        {events.map(event => (
          <div key={event.id} className="event-card">
            <h3>{event.title}</h3>
            <p>Date: {new Date(event.eventDate).toLocaleDateString()}</p>
            <p>Time: {event.startTime.slice(0, 5)} - {event.endTime.slice(0, 5)}</p>
            <p>Location: {event.location}</p>
            <p>Points: {event.points}</p>
            <button onClick={() => {
              console.log(`Navigating to event details for event ID: ${event.id}`);
              navigate(`/event_details/${event.id}`);
            }}>
              View Details
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default UserDashboard;
