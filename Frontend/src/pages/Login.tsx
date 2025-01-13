import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext'; // Adjust path as needed
import './Login.css';

const Login: React.FC = () => {
    const navigate = useNavigate();
    const { login } = useAuth();
    
    // State management
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);

    // Form submission handler
    const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setIsLoading(true);
        setMessage('');

        try {
            const response = await fetch('http://localhost:3000/api/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // Important for cookie-based authentication
                body: JSON.stringify({ 
                    Username: username, 
                    Password: password 
                }),
            });

            const data = await response.json();
            console.log('API Response:', data); // Added logging

            if (response.ok) {
                // Successful login
                login({
                    id: data.userId,
                    username: data.username,
                    isAdmin: data.isAdmin
                });

                // Navigate based on user role
                navigate('/dashboard'); // Navigate to the dashboard route
            } else {
                // Login failed
                console.error('Login failed:', data.message);
                setMessage(data.message || 'Login failed');
            }
        } catch (error) {
            // Network or unexpected error
            console.error('Login error:', error);
            setMessage('An error occurred during login. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    // Toggle password visibility
    const togglePasswordVisibility = () => {
        setShowPassword(!showPassword);
    };

    return (
        <div className="login-container">
            <form onSubmit={handleLogin} className="login-form">
                <h2>Login</h2>
                
                {/* Username Input */}
                <div className="form-group">
                    <label htmlFor="username">Username</label>
                    <input 
                        type="text" 
                        id="username"
                        value={username} 
                        onChange={(e) => setUsername(e.target.value)} 
                        placeholder="Enter your username" 
                        required 
                        disabled={isLoading}
                        autoComplete="username"
                    />
                </div>

                {/* Password Input */}
                <div className="form-group">
                    <label htmlFor="password">Password</label>
                    <div className="password-input-wrapper">
                        <input 
                            type={showPassword ? "text" : "password"}
                            id="password"
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)} 
                            placeholder="Enter your password" 
                            required 
                            disabled={isLoading}
                            autoComplete="current-password"
                        />
                        <button 
                            type="button" 
                            onClick={togglePasswordVisibility}
                            className="password-toggle"
                        >
                            {showPassword ? "Hide" : "Show"}
                        </button>
                    </div>
                </div>

                {/* Error Message */}
                {message && (
                    <div className="error-message">
                        {message}
                    </div>
                )}

                {/* Submit Button */}
                <button 
                    type="submit" 
                    disabled={isLoading}
                    className="login-button"
                >
                    {isLoading ? 'Logging in...' : 'Login'}
                </button>

                {/* Go Back Button */}
                <button 
                    type="button" 
                    onClick={() => navigate('/')}
                    className="go-back-button"
                >
                    Go Back
                </button>

                {/* Register Link */}
                <div className="register-link">
                    Don't have an account? 
                    <a href="/register">Register</a>
                </div>
            </form>
        </div>
    );
};

export default Login;
