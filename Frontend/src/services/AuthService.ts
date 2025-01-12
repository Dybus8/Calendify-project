import axios from "axios";

export const registerUser = async (userData: {
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}) => {
    const response = await axios.post("/api/login/register", userData);
    return response.data;
};
