import axios from "axios";

export const registerUser = async (userData: {
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}) => {
    const response = await axios.post("/api/register", userData);
    return response.data;
};

export const loginUser = async (credentials: {
    userName: string;
    password: string;
}) => {
    try {
        const response = await axios.post("/api/login", credentials);
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (error.response?.status === 500) {
                throw new Error("Server error occurred. Please try again later.");
            }
            throw new Error(error.response?.data?.message || "Login failed");
        }
        throw new Error("An unexpected error occurred");
    }
};
