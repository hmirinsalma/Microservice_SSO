import axiosInstance from './axiosInstance';
import { ApiResponse } from '../types';

export const getDashboard = () =>
  axiosInstance.get<ApiResponse<unknown>>('/dashboard');
