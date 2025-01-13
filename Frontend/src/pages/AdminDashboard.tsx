import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './AdminDashboard.css'; // Optional: create this file for styling

interface User {
    id: number;
    username: string;
}

const AdminDashboard: React.FC = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const [users, setUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [userInfo, setUserInfo] = useState<any>(null);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch('http://localhost:5097/api/Admin/users', {
                    method: 'GET',
                    credentials: 'include',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch users');
                }

                const data = await response.json();
                setUsers(data);
                setIsLoading(false);
            } catch (error) {
                console.error('Error fetching users:', error);
                setIsLoading(false);
            }
        };

        fetchUsers();
    }, []);

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

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="admin-dashboard">
            <div className="dashboard-container">
                <h1>Admin Dashboard</h1>
                <div className="admin-info">
                    <p>Welcome, {user?.username} (Admin)</p>
                </div>

                {userInfo ? (
                    <div>
                        <p>Username: {userInfo.username}</p>
                        <p>User ID: {userInfo.userId}</p>
                    </div>
                ) : (
                    <p>Loading user information...</p>
                )}

                <div className="user-management">
                    <h2>User Management</h2>
                    <table className="users-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Username</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map(userData => (
                                <tr key={userData.id}>
                                    <td>{userData.id}</td>
                                    <td>{userData.username}</td>
                                    <td>
                                        <button 
                                            className="btn btn-edit"
                                            onClick={() => {/* Edit user */}}
                                        >
                                            Edit
                                        </button>
                                        <button 
                                            className="btn btn-delete"
                                            onClick={() => {/* Delete user */}}
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                <div className="dashboard-actions">
                    <button 
                        className="btn btn-danger"
                        onClick={handleLogout}
                    >
                        Logout
                    </button>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;