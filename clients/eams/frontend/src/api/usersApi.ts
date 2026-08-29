import axiosInstance from './axiosInstance';
import { ApiResponse, UserDto } from '../types';

export const getUsers = () =>
  axiosInstance.get<ApiResponse<UserDto[]>>('/users');

export const getUser = (id: string) =>
  axiosInstance.get<ApiResponse<UserDto>>(`/users/${id}`);

export const createUser = (data: unknown) =>
  axiosInstance.post<ApiResponse<UserDto>>('/users', data);

export const updateUser = (id: string, data: unknown) =>
  axiosInstance.put<ApiResponse<UserDto>>(`/users/${id}`, data);

export const toggleActive = (id: string) =>
  axiosInstance.patch(`/users/${id}/activate`);

export const getProfile = () =>
  axiosInstance.get<ApiResponse<UserDto>>('/profile');

export const updateProfile = (data: { telephone?: string; photoUrl?: string }) =>
  axiosInstance.put<ApiResponse<UserDto>>('/profile', data);

// NOTE SSO : changePassword supprimée — le changement de mot de passe
// sera géré par le microservice SSO après intégration.
