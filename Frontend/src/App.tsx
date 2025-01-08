import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Home from './pages/Home'; // Import your Home component
import Login from './pages/Login'; // Import your Login component
import UserDashboard from './pages/UserDashboard'; // Import your User Dashboard component
import AdminDashboard from './pages/AdminDashboard'; // Import your Admin Dashboard component
import ErrorPage from './shared/ErrorPage'; // Import your Error Page component
import { useAuth } from './context/AuthContext'; // Import your Auth context

const App: React.FC = () => {
    const { isAuthenticated } = useAuth(); // Get authentication status from context

    return (
        <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />

            {/* Protected User Routes */}
            <Route 
                path="/user/dashboard" 
                element={isAuthenticated ? <UserDashboard /> : <Navigate to="/login" />} 
            />

            {/* Protected Admin Routes */}
            <Route 
                path="/admin/dashboard" 
                element={isAuthenticated ? <AdminDashboard /> : <Navigate to="/login" />} 
            />

            {/* Catch-all Error Route */}
            <Route path="*" element={<ErrorPage />} />
        </Routes>
    );
};

export default App;