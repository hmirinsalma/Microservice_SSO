import React from 'react';
import authService from '../../auth/authService';

const LoginSSO = () => {
  const handleLogin = () => {
    authService.login();
  };

  return (
    <div style={{ 
      display: 'flex', 
      flexDirection: 'column', 
      alignItems: 'center', 
      justifyContent: 'center', 
      height: '100vh',
      backgroundColor: '#f5f5f5'
    }}>
      <h1>TIMS - ONEE</h1>
      <h2>Time & Incident Management System</h2>
      <p>Connectez-vous avec votre compte ONEE SSO</p>
      <button 
        onClick={handleLogin}
        style={{
          padding: '12px 24px',
          fontSize: '16px',
          backgroundColor: '#28a745',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        🔐 Se connecter avec ONEE SSO
      </button>
    </div>
  );
};

export default LoginSSO;
