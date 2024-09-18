import React, { useState } from 'react';
import axios from 'axios';

const Login: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState<string | null>(null);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage(null);

        console.log("Attempting login with:", { username, password }); // Log the login attempt

        try {
            const response = await axios.post('http://localhost:5000/api/v1/Login/Login', {
                username,
                password
            });

            console.log("Response:", response); // Log the response

            if (response.status === 200) {
                setMessage('Login successful');
                // Redirect to another controller after successful login
                window.location.href = 'http://localhost:5000/api/v1/Dashboard';
            }
        } catch (error) {
            console.log("Error response:", error); // Log the error response

            if (axios.isAxiosError(error) && error.response) {
                setMessage(error.response.data);
            } else {
                setMessage('An error occurred during login');
            }
        }
    };

    return (
        <div>
            <h2>Admin Login</h2>
            <form onSubmit={handleLogin}>
                <div>
                    <label>Username:</label>
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Login</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default Login;
