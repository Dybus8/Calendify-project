import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useParams } from 'react-router-dom';
import './EventDetails.css';
import { submitReview, getEventReviews } from '../services/ReviewService';

const EventDetails = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { eventId } = useParams<{ eventId: string }>();
  const [event, setEvent] = useState(null);
  const [reviews, setReviews] = useState([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [newReview, setNewReview] = useState({
    rating: 5,
    comment: ''
  });

  const mockAttendees = [
    'John Doe',
    'Jane Smith',
    'Alice Johnson',
    'Bob Brown'
  ];

  useEffect(() => {
    const fetchEventDetails = async () => {
      try {
        const response = await fetch(`/api/events/${eventId}`);
        if (!response.ok) throw new Error('Event not found');
        const data = await response.json();
        setEvent(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unknown error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchEventDetails();
  }, [eventId]);

  const handleSubmitReview = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const reviewData = { rating: newReview.rating, comment: newReview.comment };
      const reviewResponse = await submitReview(eventId!, reviewData);
      setReviews([...reviews, reviewResponse]);
      setNewReview({ rating: 5, comment: '' });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to submit review');
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!event) return <div>Event not found</div>;

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
          <p>Date: {event.eventDate}</p>
          <p>Location: {event.location}</p>
          <button onClick={() => alert('Attend Event clicked!')}>Attend Event</button>
        </div>

        <div className="reviews-section">
          <h2>Reviews</h2>
          <form onSubmit={handleSubmitReview} className="review-form">
            <h3>Write a Review</h3>
            <div className="form-group">
              <label htmlFor="rating">Rating (1-5):</label>
              <select
                id="rating"
                value={newReview.rating}
                onChange={(e) => setNewReview({ ...newReview, rating: parseInt(e.target.value) })}
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
                onChange={(e) => setNewReview({ ...newReview, comment: e.target.value })}
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

        <div className="attendees-box">
          <h2>Attendees</h2>
          <ul>
            {mockAttendees.map((attendee, index) => (
              <li key={index}>{attendee}</li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
};

export default EventDetails;
