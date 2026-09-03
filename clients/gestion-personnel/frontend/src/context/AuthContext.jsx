import { createContext, useContext, useState, useCallback, useEffect } from 'react';
import authService from '../auth/authService';
import authApi from '../api/authApi';

/**
 * Contexte d'authentification SSO intégré.
 * 
 * Mode SSO activé : login() redirige vers ONEE.SSO
 * Compatible avec l'ancien système local en fallback
 */
const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Charger l'utilisateur SSO au démarrage
  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      const ssoUser = await authService.getUser();
      console.log('🔍 AuthContext: SSO User:', ssoUser);
      
      if (ssoUser && !ssoUser.expired) {
        // Utilisateur SSO authentifié
        // ✅ FIX DASHBOARD 403: Les rôles peuvent être un tableau ou une valeur unique
        const rolesRaw = ssoUser.profile.role;
        const roles = Array.isArray(rolesRaw) ? rolesRaw : (rolesRaw ? [rolesRaw] : []);
        
        const permissionsRaw = ssoUser.profile.permission;
        const permissions = Array.isArray(permissionsRaw) ? permissionsRaw : (permissionsRaw ? [permissionsRaw] : []);
        
        // ✅ FIX: Gérer le cas où name/email sont des arrays
        const nameRaw = ssoUser.profile.name || ssoUser.profile.email;
        const username = Array.isArray(nameRaw) ? nameRaw[0] : nameRaw;
        
        const emailRaw = ssoUser.profile.email;
        const email = Array.isArray(emailRaw) ? emailRaw[0] : emailRaw;
        
        // ✅ Mapping des rôles SSO vers rôles RH locaux
        const mapSsoRoleToRhRole = (role) => {
          // ✅ FIX: Extraire le nom du rôle si format qualifié (Role@ClientId)
          const roleName = role.includes('@') ? role.split('@')[0] : role;
          const normalized = roleName.toLowerCase();
          
          if (normalized === 'chefservice') return 'ChefDeService';
          if (normalized === 'chef_de_service') return 'ChefDeService';
          if (normalized === 'directeurressources') return 'Directeur';
          if (normalized === 'directeur') return 'Directeur';
          if (normalized === 'administrateurrh') return 'AdministrateurRH';
          if (normalized === 'employe') return 'Employe';
          return roleName; // Si déjà au bon format
        };
        
        // ✅ FIX: Filtrer pour ne prendre QUE le rôle RH (avec @gestion-personnel)
        const rhRoles = roles.filter(r => r.includes('@gestion-personnel'));
        console.log('🔍 AuthContext: Tous les rôles =', roles, ', Rôles RH =', rhRoles);
        
        const mappedRoles = (rhRoles.length > 0 ? rhRoles : roles).map(mapSsoRoleToRhRole);
        const primaryRole = mappedRoles[0] || 'Employe';
        
        const u = {
          username: username,
          email: email,
          role: primaryRole, // Premier rôle mappé
          roles: mappedRoles, // Tous les rôles mappés
          permissions: permissions,
          expiresAt: ssoUser.expires_at,
        };
        console.log('✅ AuthContext: User set:', u);
        setUser(u);
        localStorage.setItem('user', JSON.stringify(u));
      } else {
        // Fallback sur ancien système local
        console.log('⚠️ AuthContext: No SSO user, checking localStorage');
        try {
          const localUser = JSON.parse(localStorage.getItem('user'));
          if (localUser) {
            console.log('✅ AuthContext: Local user found:', localUser);
            setUser(localUser);
          } else {
            console.log('❌ AuthContext: No user found');
            setUser(null);
          }
        } catch {}
      }
    } catch (err) {
      console.error('❌ Erreur chargement utilisateur SSO:', err);
      setUser(null);
    } finally {
      setLoading(false);
    }
  };

  const login = useCallback(async (credentials) => {
    setLoading(true);
    setError(null);
    
    // Si credentials fournis = mode stub/local (fallback)
    if (credentials) {
      try {
        const { data } = await authApi.login(credentials);
        localStorage.setItem('token', data.token);
        const u = {
          username: data.username,
          email: data.email,
          role: data.role,
          expiresAt: data.expiresAt,
        };
        localStorage.setItem('user', JSON.stringify(u));
        setUser(u);
        return true;
      } catch (err) {
        setError(err.response?.data?.message || 'Email ou mot de passe incorrect.');
        return false;
      } finally {
        setLoading(false);
      }
    } else {
      // Mode SSO : redirection vers ONEE.SSO
      try {
        await authService.login();
        return true;
      } catch (err) {
        setError('Erreur lors de la redirection SSO');
        setLoading(false);
        return false;
      }
    }
  }, []);

  const logout = useCallback(async () => {
    try {
      // Déconnexion SSO
      await authService.logout();
    } catch {
      // Fallback local
      try { await authApi.logout(); } catch {}
    }
    
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider value={{
      user,
      loading,
      error,
      login,
      logout,
      refreshUser: loadUser, // ✅ Exposer la fonction pour rafraîchir l'utilisateur
      isAuthenticated: !!user,
      isAdmin: user?.role === 'AdministrateurRH' || user?.roles?.includes('AdministrateurRH'),
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
