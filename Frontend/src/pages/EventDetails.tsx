import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useParams } from 'react-router-dom';
import './EventDetails.css';
import { submitReview, getEventReviews } from '../services/ReviewService';

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

interface ReviewDTO {
  id: number;
  rating: number;
  comment: string;
  createdDate: string;
  userName: string;
}

interface NewReviewData {
  rating: number;
  comment: string;
}

const EventDetails = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { eventId } = useParams<{ eventId: string }>();
  const [event, setEvent] = useState<Event | null>(null);
  const [reviews, setReviews] = useState<ReviewDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [newReview, setNewReview] = useState<NewReviewData>({
    rating: 5,
    comment: ''
  });

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

    const fetchData = async () => {
      try {
        // Fetch event data
        const response = await fetch(`/api/events/${eventId}`);
        if (!response.ok) throw new Error('Failed to fetch event');
        const data = await response.json();
        setEvent(data);

        // Fetch reviews using the new service
        const reviewsData = await getEventReviews(eventId!);
        setReviews(reviewsData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unknown error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [user, navigate, eventId]);

  const handleAttendEvent = async () => {
    if (!event) return;

    const eventDate = new Date(event.date);
    if (eventDate < new Date()) {
      setError('Cannot attend past events.');
      return;
    }

    if (event.attendeesCount >= 100) {
      setError('This event is at capacity.');
      return;
    }

    try {
      const response = await fetch(`/api/events/${event.id}/attend`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        if (response.status === 500) {
          setError('Server error occurred. Please try again later.');
        } else {
          setError('Failed to attend event');
        }
        return;
      }

      const updatedEvent = await response.json();
      setEvent(updatedEvent);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('An unknown error occurred');
      }
    }
  };

  const handleSubmitReview = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!event || !eventId) return;

    try {
        const reviewData = {
            rating: newReview.rating,
            comment: newReview.comment
        };
        const reviewResponse = await submitReview(eventId, reviewData);
        setReviews([...reviews, reviewResponse]);
        setNewReview({ rating: 5, comment: '' });
    } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to submit review');
    }
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
          <button onClick={handleAttendEvent}>Attend Event</button>
        </div>
        <div className="reviews-section">
          <h2>Reviews</h2>
          
          {/* Add Review Form */}
          <form onSubmit={handleSubmitReview} className="review-form">
            <h3>Write a Review</h3>
            <div className="form-group">
              <label htmlFor="rating">Rating (1-5):</label>
              <select
                id="rating"
                value={newReview.rating}
                onChange={(e) => setNewReview({
                  ...newReview,
                  rating: parseInt(e.target.value)
                })}
              >
                {[1, 2, 3, 4, 5].map(num => (
                  <option key={num} value={num}>{num}</option>
                ))}
              </select>
            </div>
            
            <div className="form-group">
              <label htmlFor="comment">Comment:</label>
              <textarea
                id="comment"
                value={newReview.comment}
                onChange={(e) => setNewReview({
                  ...newReview,
                  comment: e.target.value
                })}
                required
              />
            </div>
            
            <button type="submit">Submit Review</button>
          </form>

          {reviews.length > 0 ? (
            reviews.map(review => (
              <div key={review.id} className="review">
                <p><strong>{review.userName}</strong> - Rating: {review.rating}</p>
                <p>{review.comment}</p>
              </div>
            ))
          ) : (
            <p>No reviews yet.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default EventDetails;
