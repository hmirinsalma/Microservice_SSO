import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5137/api';

const apiClientSSO: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - Ajouter le token + custom headers EAMS
apiClientSSO.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    const token = await authService.getAccessToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // ⭐ Ajouter les custom claims EAMS dans les headers
    const eamsContext = await authService.getEamsContext();
    if (eamsContext.userId && config.headers) {
      config.headers['X-EAMS-User-Id'] = eamsContext.userId;
    }
    if (eamsContext.serviceId && config.headers) {
      config.headers['X-EAMS-Service-Id'] = eamsContext.serviceId;
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClientSSO.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    if (error.response?.status === 401) {
      console.warn('⚠️ Token expiré, redirection vers login');
      await authService.logout();
    }
    return Promise.reject(error);
  }
);

export default apiClientSSO;
