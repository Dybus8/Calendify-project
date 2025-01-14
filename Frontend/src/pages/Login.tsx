import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext'; // Adjust path as needed
import './Login.css';

const Login: React.FC = () => {
    const navigate = useNavigate();
    const { login } = useAuth();
    
    // State management
    const [userName, setUserName] = useState('');  // Changed from username to userName
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
            await login(userName, password);  // Changed from username to userName
            navigate('/dashboard'); // Navigate to the dashboard route
        } catch (error) {
            if (error instanceof Error) {
                setMessage(error.message);
            } else {
                setMessage('An unexpected error occurred');
            }
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
                    <label htmlFor="userName">Username</label>
                    <input 
                        type="text" 
                        id="userName"
                        value={userName} 
                        onChange={(e) => setUserName(e.target.value)} 
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
