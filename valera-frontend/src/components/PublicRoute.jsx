import React from 'react';
import { Navigate } from 'react-router-dom';
import { getCurrentUser } from '../services/api';

const PublicRoute = ({ children, isAuthenticated }) => {
    if (isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    return children;
};

export default PublicRoute;
