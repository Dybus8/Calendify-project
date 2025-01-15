import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

// Define the structure of the event details
interface EventDetails {
    title: string;
    description: string;
    location: string;
    eventDate: string; 
}

const EventDetails = () => {
    const { eventId } = useParams<{ eventId?: string }>(); // Make eventId optional
    const [eventDetails, setEventDetails] = useState<EventDetails | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchEventDetails = async () => {
            if (!eventId) {
                setError('Event ID is required');
                return;
            }

            const id = parseInt(eventId, 10);
            if (isNaN(id) || id < 1 || id > 10) {
                setError('Invalid event ID');
                return;
            }

            try {
                const response = await axios.get(`/api/events/${id}`);
                setEventDetails(response.data);
            } catch (err) {
                setError('Event not found');
            }
        };

        fetchEventDetails();
    }, [eventId]);

    if (error) {
        return <div>{error}</div>;
    }

    if (!eventDetails) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h1>{eventDetails.title}</h1>
            <p>{eventDetails.description}</p>
            <p>Location: {eventDetails.location}</p>
            <p>Date: {eventDetails.eventDate}</p>
        </div>
    );
};

export default EventDetails;
