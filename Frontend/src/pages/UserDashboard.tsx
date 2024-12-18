import React from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './UserDashboard.css'; // Optional: create this file for styling

const UserDashboard: React.FC = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <div className="user-dashboard">
            <div className="dashboard-container">
                <h1>Welcome, {user?.username}!</h1>
                <div className="dashboard-content">
                    <h2>User Dashboard</h2>
                    <div className="user-info">
                        <p>Username: {user?.username}</p>
                        <p>User ID: {user?.id}</p>
                    </div>
                    <div className="dashboard-actions">
                        <button 
                            className="btn btn-primary"
                            onClick={() => navigate('/profile')}
                        >
                            Edit Profile
                        </button>
                        <button 
                            className="btn btn-danger"
                            onClick={handleLogout}
                        >
                            Logout
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default UserDashboard;