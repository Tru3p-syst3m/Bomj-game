import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { getCurrentUser } from './services/api';
import ValeraList from './components/ValeraList/ValeraList';
import ValeraStats from './components/ValeraStats/ValeraStats';
import Login from './components/Login';
import Register from './components/Register';
import ProtectedRoute from './components/ProtectedRoute';
import PublicRoute from './components/PublicRoute';
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuth = () => {
      const user = getCurrentUser();
      console.log('Проверка аутентификации, пользователь:', user);
      setIsAuthenticated(!!user);
      setIsLoading(false);
    };

    checkAuth();

    window.addEventListener('storage', checkAuth);
    window.addEventListener('authChange', checkAuth);

    return () => {
      window.removeEventListener('storage', checkAuth);
      window.removeEventListener('authChange', checkAuth);
    };
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    window.dispatchEvent(new Event('authChange'));
  };

  if (isLoading) {
    return <div className="loading">Проверка аутентификации...</div>;
  }

  console.log('App render, isAuthenticated:', isAuthenticated);

  return (
    <Router>
      <div className="App">
        <Routes>
          {/* Перенаправление с корня на /my */}
          <Route path="/" element={<Navigate to="/my" replace />} />

          {/* Публичные маршруты */}
          <Route path="/login" element={
            <PublicRoute isAuthenticated={isAuthenticated}>
              <Login setIsAuthenticated={setIsAuthenticated} />
            </PublicRoute>
          } />

          <Route path="/register" element={
            <PublicRoute isAuthenticated={isAuthenticated}>
              <Register setIsAuthenticated={setIsAuthenticated} />
            </PublicRoute>
          } />

          {/* Защищенные маршруты */}
          <Route path="/my" element={
            <ProtectedRoute isAuthenticated={isAuthenticated}>
              <ValeraList onLogout={handleLogout} />
            </ProtectedRoute>
          } />

          <Route path="/valera/:id" element={
            <ProtectedRoute isAuthenticated={isAuthenticated}>
              <ValeraStats onLogout={handleLogout} />
            </ProtectedRoute>
          } />
        </Routes>
      </div>
    </Router>
  );
}

export default App;