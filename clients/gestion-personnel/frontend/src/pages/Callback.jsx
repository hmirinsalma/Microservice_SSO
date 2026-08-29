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
        
        // Rafraîchir le contexte d'authentification
        await refreshUser();
        console.log('✅ Callback: User context refreshed');
        
        // Rediriger vers le dashboard (route = "/")
        navigate('/', { replace: true });
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
