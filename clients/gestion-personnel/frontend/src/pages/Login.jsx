import React from 'react';
import authService from '../auth/authService';

const Login = () => {
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
      <div style={{
        backgroundColor: 'white',
        padding: '40px',
        borderRadius: '8px',
        boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
        textAlign: 'center'
      }}>
        <h1 style={{ color: '#0066cc', marginBottom: '20px' }}>
          Gestion Personnel - ONEE
        </h1>
        <p style={{ color: '#666', marginBottom: '30px' }}>
          Connectez-vous avec votre compte ONEE SSO
        </p>
        <button 
          onClick={handleLogin}
          style={{
            padding: '12px 24px',
            fontSize: '16px',
            backgroundColor: '#0066cc',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            transition: 'background-color 0.3s'
          }}
          onMouseOver={(e) => e.target.style.backgroundColor = '#0052a3'}
          onMouseOut={(e) => e.target.style.backgroundColor = '#0066cc'}
        >
          🔐 Se connecter avec ONEE SSO
        </button>
      </div>
    </div>
  );
};

export default Login;
