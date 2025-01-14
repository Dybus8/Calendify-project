import axios from 'axios';

interface ReviewData {
    rating: number;
    comment: string;
}

export const submitReview = async (eventId: string, reviewData: ReviewData) => {
    try {
        const response = await axios.post(`/api/Reviews/${eventId}`, reviewData);
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(error.response?.data?.message || 'Failed to submit review');
        }
        throw new Error('Failed to submit review');
    }
};

export const getEventReviews = async (eventId: string) => {
    try {
        const response = await axios.get(`/api/Reviews/${eventId}/reviews`);  // Updated endpoint
        return response.data;
    } catch (error) {
        throw new Error('Failed to fetch reviews');
    }
};
