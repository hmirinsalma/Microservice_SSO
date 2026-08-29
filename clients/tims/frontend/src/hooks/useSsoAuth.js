import { useState, useEffect } from 'react';
import authService from '../auth/authService';

/**
 * Hook personnalisé pour gérer l'authentification SSO dans les composants
 * 
 * @returns {Object} État d'authentification et méthodes
 */
export const useSsoAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [timsContext, setTimsContext] = useState(null);

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    try {
      const authenticated = await authService.isAuthenticated();
      setIsAuthenticated(authenticated);

      if (authenticated) {
        const profile = await authService.getUserProfile();
        const context = await authService.getTimsContext();
        setUser(profile);
        setTimsContext(context);
      }
    } catch (error) {
      console.error('❌ Erreur lors de la vérification de l\'authentification:', error);
      setIsAuthenticated(false);
      setUser(null);
      setTimsContext(null);
    } finally {
      setLoading(false);
    }
  };

  const login = () => {
    authService.login();
  };

  const logout = async () => {
    await authService.logout();
    setIsAuthenticated(false);
    setUser(null);
    setTimsContext(null);
  };

  const hasRole = async (role) => {
    return await authService.hasRole(role);
  };

  const hasPermission = async (permission) => {
    return await authService.hasPermission(permission);
  };

  const getAccessToken = async () => {
    return await authService.getAccessToken();
  };

  return {
    isAuthenticated,
    user,
    loading,
    timsContext,
    login,
    logout,
    hasRole,
    hasPermission,
    getAccessToken,
    refreshAuth: checkAuth
  };
};

export default useSsoAuth;
