import axiosInstance from './axiosInstance';
import { ApiResponse } from '../types';

export const login = (email: string, password: string) =>
  axiosInstance.post<ApiResponse<{ token: string; role: string; nom: string; prenom: string; email: string; userId: string; serviceId?: string; expiresAt: string }>>('/auth/login', { email, password });

export const logout = () =>
  axiosInstance.post('/auth/logout');
