import axios from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5115/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - Ajouter le token + custom headers TIMS
apiClient.interceptors.request.use(
  async (config) => {
    const token = await authService.getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // ⭐ Ajouter les custom claims TIMS dans les headers
    const timsContext = await authService.getTimsContext();
    if (timsContext.userId) {
      config.headers['X-TIMS-User-Id'] = timsContext.userId;
    }
    if (timsContext.serviceId) {
      config.headers['X-TIMS-Service-Id'] = timsContext.serviceId;
    }
    if (timsContext.teamId) {
      config.headers['X-TIMS-Team-Id'] = timsContext.teamId;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn('⚠️ Token expiré, redirection vers login');
      await authService.logout();
    }
    return Promise.reject(error);
  }
);

export default apiClient;
