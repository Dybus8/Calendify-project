import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useParams } from 'react-router-dom';
import './AttendEvent.css';

const AttendEvent = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { eventId } = useParams<{ eventId: string }>(); // Ensure eventId is extracted
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      navigate('/login');
      return;
    }

    if (!eventId) {
      setError('Event ID is missing');
      return;
    }

    const attendEvent = async () => {
      try {
        console.log(`Attending event with ID: ${eventId}`); // Log event ID
        const response = await fetch(`/api/events/${eventId}/attend`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (!response.ok) {
          if (response.status === 500) {
            setError('Server error occurred. Please try again later.');
          } else {
            const errorData = await response.json();
            setError(errorData.message || 'Failed to attend event');
          }
          return;
        }

        const data = await response.json();
        console.log('Successfully attended event:', data); // Log success
        setMessage(data.message);
      } catch (err) {
        console.error('Error attending event:', err); // Log error
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError('An unknown error occurred');
        }
      }
    };

    attendEvent();
  }, [user, navigate, eventId]);

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div className="attend-event-page">
      {message ? (
        <div>
          <p>{"Succes!!!"}</p>
          <button onClick={() => navigate('/dashboard')}>Back to Dashboard</button>
        </div>
      ) : (
        <div>Loading...</div>
      )}
    </div>
  );
};

export default AttendEvent;
