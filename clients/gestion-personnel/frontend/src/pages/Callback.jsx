import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../auth/authService';
import { useAuth } from '../context/AuthContext';

const Callback = () => {
  const navigate = useNavigate();
  const { refreshUser } = useAuth();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        const user = await authService.completeLogin();
        console.log('✅ Callback: Authentication successful, user:', user);
        
        // 🎯 AUTO-PROVISIONING: Appeler le backend pour créer l'utilisateur si nécessaire
        console.log('📡 Calling /api/auth/sso-callback for auto-provisioning...');
        
        try {
          const response = await fetch('http://localhost:5291/api/auth/sso-callback', {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${user.access_token}`,
              'Content-Type': 'application/json'
            }
          });

          if (response.ok) {
            const provisioningResult = await response.json();
            console.log('✅ Auto-provisioning successful:', provisioningResult);
          } else {
            console.warn('⚠️ Auto-provisioning returned non-OK status:', response.status);
          }
        } catch (provisioningError) {
          console.error('⚠️ Auto-provisioning error (continuing anyway):', provisioningError);
        }
        
        // Rafraîchir le contexte d'authentification
        await refreshUser();
        console.log('✅ Callback: User context refreshed');
        
        // Naviguer vers le dashboard approprié selon le rôle
        const userRole = user.profile?.role;
        let dashboardPath = '/dashboard-employe'; // Par défaut
        
        // Gérer les multi-rôles (choisir le premier rôle RH valide)
        const roleValue = Array.isArray(userRole) 
          ? userRole.find(r => r === 'ChefService' || r === 'DirecteurRessources' || r === 'AdministrateurRH' || r === 'Employe')
          : userRole;
        
        console.log(`🎯 [RH] User role detected: ${roleValue} (from: ${JSON.stringify(userRole)})`);
        
        if (roleValue === 'DirecteurRessources') {
          dashboardPath = '/dashboard-directeur';
        } else if (roleValue === 'ChefService') {
          dashboardPath = '/dashboard-chef';
        } else if (roleValue === 'AdministrateurRH') {
          dashboardPath = '/dashboard-admin';
        } else {
          dashboardPath = '/dashboard-employe';
        }
        
        console.log(`🚀 [RH] Navigating to: ${dashboardPath}`);
        window.location.href = dashboardPath;
      } catch (error) {
        console.error('❌ Callback: Error during login:', error);
        navigate('/login', { replace: true });
      }
    };

    completeLogin();
  }, [navigate, refreshUser]);

  return (
    <div style={{ 
      textAlign: 'center', 
      marginTop: '100px',
      fontSize: '18px'
    }}>
      <h2>🔄 Authentification en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
