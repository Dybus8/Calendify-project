import React, { createContext, useContext, useState, ReactNode } from 'react';
import axios from 'axios'; // Import axios
import { loginUser } from '../services/AuthService';

// Configure axios with base URL
axios.defaults.baseURL = 'http://localhost:3000'; // Adjust this port to match your backend

interface User {
  id: number;
  userName: string;  // Changed from username to userName
  isAdmin: boolean;
  firstName: string; // Add firstName
  lastName: string;  // Add lastName
  points: number;    // Add points
}

interface AuthContextType {
  user: User | null;
  login: (userName: string, password: string) => Promise<void>;  // Changed username to userName
  logout: () => void;
}

interface AuthProviderProps {
  children: ReactNode;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);

  const login = async (userName: string, password: string) => {  // Changed username to userName
    try {
      console.log('Attempting login for user:', userName);
      const response = await axios.post('http://localhost:3000/api/login', { 
        userName, 
        password 
      }, {
        headers: {
          'Content-Type': 'application/json'
        }
      });

      const userData = response.data.user;
      // Remove password from logged user data
      const { password: _, ...safeUserData } = userData;
      
      console.log('Login successful for:', safeUserData);
      setUser(safeUserData); // Ensure safeUserData includes firstName and lastName
    } catch (error) {
      console.error('Login failed');
      if (axios.isAxiosError(error)) {
        if (error.response) {
          console.error('Error:', error.response.data.message || 'Unknown error');
          alert(`Login failed: ${error.response.data.message || 'Internal Server Error'}`);
        } else {
          alert('Login failed: No response from server');
        }
      } else {
        alert('Login failed: An unexpected error occurred');
      }
    }
  };

  const logout = () => {
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
