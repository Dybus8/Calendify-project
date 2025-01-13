import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './UserDashboard.css'; // Optional: create this file for styling

const UserDashboard: React.FC = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const [userInfo, setUserInfo] = useState<any>(null);

    useEffect(() => {
        const fetchUserInfo = async () => {
            try {
                const response = await fetch('http://localhost:3000/api/Login/userinfo', {
                    method: 'GET',
                    credentials: 'include',
                });
                const data = await response.json();
                setUserInfo(data);
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        };

        fetchUserInfo();
    }, []);

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
                        {userInfo ? (
                            <div>
                                <p>Username: {userInfo.username}</p>
                                <p>User ID: {userInfo.userId}</p>
                            </div>
                        ) : (
                            <p>Loading user information...</p>
                        )}
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