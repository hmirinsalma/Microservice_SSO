import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import authService from '../auth/authService';

const SsoAuthContext = createContext(null);

/**
 * Provider pour gérer l'authentification SSO dans toute l'application
 * Alternative à AuthContext.jsx pour une migration complète vers SSO
 */
export function SsoAuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [timsContext, setTimsContext] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);

  // Charger l'utilisateur au démarrage
  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = useCallback(async () => {
    try {
      const authenticated = await authService.isAuthenticated();
      setIsAuthenticated(authenticated);

      if (authenticated) {
        const profile = await authService.getUserProfile();
        const context = await authService.getTimsContext();
        
        setUser(profile);
        setTimsContext(context);

        console.log('✅ SSO User loaded in context:', {
          name: profile?.name,
          email: profile?.email,
          roles: profile?.roles,
          timsContext: context
        });
      }
    } catch (error) {
      console.error('❌ Erreur lors du chargement de l\'utilisateur SSO:', error);
      setIsAuthenticated(false);
      setUser(null);
      setTimsContext(null);
    } finally {
      setLoading(false);
    }
  }, []);

  const login = useCallback(() => {
    return authService.login();
  }, []);

  const logout = useCallback(async () => {
    try {
      await authService.logout();
      setIsAuthenticated(false);
      setUser(null);
      setTimsContext(null);
    } catch (error) {
      console.error('❌ Erreur lors de la déconnexion:', error);
    }
  }, []);

  const hasRole = useCallback((role) => {
    if (!user?.roles) return false;
    return user.roles.includes(role);
  }, [user]);

  const hasAnyRole = useCallback((roles) => {
    if (!user?.roles) return false;
    return roles.some(role => user.roles.includes(role));
  }, [user]);

  const hasPermission = useCallback(async (permission) => {
    return await authService.hasPermission(permission);
  }, []);

  const getAccessToken = useCallback(async () => {
    return await authService.getAccessToken();
  }, []);

  // Getters pour les custom claims TIMS
  const timsUserId = timsContext?.userId;
  const timsServiceId = timsContext?.serviceId;
  const timsTeamId = timsContext?.teamId;

  const value = {
    // État
    user,
    isAuthenticated,
    loading,
    timsContext,
    
    // Custom claims TIMS
    timsUserId,
    timsServiceId,
    timsTeamId,

    // Méthodes
    login,
    logout,
    hasRole,
    hasAnyRole,
    hasPermission,
    getAccessToken,
    refreshUser: loadUser,

    // Rôles (compatibilité avec l'ancien AuthContext)
    role: user?.roles?.[0] || null,
    roles: user?.roles || [],
  };

  return (
    <SsoAuthContext.Provider value={value}>
      {children}
    </SsoAuthContext.Provider>
  );
}

/**
 * Hook pour utiliser le contexte d'authentification SSO
 */
export const useSsoAuthContext = () => {
  const context = useContext(SsoAuthContext);
  if (!context) {
    throw new Error('useSsoAuthContext must be used within SsoAuthProvider');
  }
  return context;
};

export default SsoAuthContext;
