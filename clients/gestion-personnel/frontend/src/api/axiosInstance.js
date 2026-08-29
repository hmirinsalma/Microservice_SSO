import axios from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5291/api';

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

// Attacher le token SSO JWT à chaque requête
axiosInstance.interceptors.request.use(
  async (config) => {
    // Priorité au token SSO
    const ssoToken = await authService.getAccessToken();
    if (ssoToken) {
      config.headers.Authorization = `Bearer ${ssoToken}`;
    } else {
      // Fallback sur token local (si mode stub encore actif)
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Gérer les erreurs globalement (401 → redirection login SSO)
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn('⚠️ Token expiré ou invalide, redirection vers SSO');
      // Nettoyer les anciennes données
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      // Redirection vers login SSO
      await authService.logout();
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
