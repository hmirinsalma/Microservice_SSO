import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import authService from '../../auth/authService';

const ProtectedRoute = ({ children, requiredRole, requiredPermission }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(null);
  const [hasAccess, setHasAccess] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await authService.isAuthenticated();
      setIsAuthenticated(authenticated);

      if (authenticated) {
        if (requiredRole) {
          const hasRole = await authService.hasRole(requiredRole);
          setHasAccess(hasRole);
        }

        if (requiredPermission) {
          const hasPermission = await authService.hasPermission(requiredPermission);
          setHasAccess(hasPermission);
        }
      }
    };

    checkAuth();
  }, [requiredRole, requiredPermission]);

  if (isAuthenticated === null) {
    return <div>Chargement...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!hasAccess) {
    return <div>⛔ Accès refusé - Permissions insuffisantes</div>;
  }

  return children;
};

export default ProtectedRoute;
