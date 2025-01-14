import React, { createContext, useContext, useState, ReactNode } from 'react';
import { loginUser } from '../services/AuthService';

interface User {
  id: number;
  userName: string;  // Changed from username to userName
  isAdmin: boolean;
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
      const result = await loginUser({ userName, password });
      setUser(result.user);
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  };

  const logout = () => {
    setUser(null);
    // Add any additional logout logic here
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
