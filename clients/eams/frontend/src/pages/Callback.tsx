import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../auth/authService';

const Callback: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        const user = await authService.completeLogin();
        
        // Récupérer les custom claims EAMS
        const eamsContext = await authService.getEamsContext();
        console.log('🎯 EAMS Context:', eamsContext);
        
        navigate('/dashboard');
      } catch (error) {
        console.error('❌ Erreur lors du callback:', error);
        navigate('/login');
      }
    };

    completeLogin();
  }, [navigate]);

  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h2>Authentification EAMS en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
