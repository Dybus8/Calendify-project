import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import UserDashboard from './pages/UserDashboard';
import AdminDashboard from './pages/AdminDashboard';
import { useAuth } from './context/AuthContext';
import EventDetails from './pages/EventDetails';

function App() {
  const { user } = useAuth();

  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/dashboard"
        element={
          user ? (
            (console.log('User role:', user.isAdmin), user.isAdmin ? (
              <AdminDashboard />
            ) : (
              <UserDashboard />
            ))
          ) : (
            <Navigate to="/login" />
          )
        }
      />
      <Route
        path="/event_details/:id"
        element={
          user ? (
            (console.log('User role:', user.isAdmin), user.isAdmin ? (
              <EventDetails /> // MAAK HIER EEN ADMIN EVENTDETAILS VAN
            ) : (
              <EventDetails />
            ))
          ) : (
            <Navigate to="/login" />
          )
        }
      />
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  );
}

export default App;
