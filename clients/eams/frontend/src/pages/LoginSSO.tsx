import React from 'react';
import authService from '../auth/authService';

const LoginSSO: React.FC = () => {
  const handleLogin = () => {
    // ✅ Vider le localStorage avant la redirection SSO
    localStorage.clear();
    sessionStorage.clear();
    
    // ✅ Redirection manuelle vers le SSO (comme TIMS)
    const ssoUrl = 'http://localhost:5205/connect/authorize';
    const params = new URLSearchParams({
      client_id: 'eams-spa',
      redirect_uri: 'http://localhost:5173/auth/callback',
      response_type: 'code',
      scope: 'openid profile email roles offline_access',
      state: Math.random().toString(36).substring(7),
    });
    
    window.location.href = `${ssoUrl}?${params.toString()}`;
  };

  return (
    <div style={{ 
      display: 'flex', 
      flexDirection: 'column', 
      alignItems: 'center', 
      justifyContent: 'center', 
      height: '100vh',
      backgroundColor: '#f0f2f5'
    }}>
      <h1>EAMS - ONEE</h1>
      <h2>Equipment & Asset Management System</h2>
      <p>Connectez-vous avec votre compte ONEE SSO</p>
      <button 
        onClick={handleLogin}
        style={{
          padding: '12px 24px',
          fontSize: '16px',
          backgroundColor: '#1890ff',
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
