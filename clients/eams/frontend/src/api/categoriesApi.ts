import axiosInstance from './axiosInstance';
import { ApiResponse, CategorieDto } from '../types';

export const getCategories = () =>
  axiosInstance.get<ApiResponse<CategorieDto[]>>('/categories');

export const getCategorie = (id: string) =>
  axiosInstance.get<ApiResponse<CategorieDto>>(`/categories/${id}`);

export const createCategorie = (data: unknown) =>
  axiosInstance.post<ApiResponse<CategorieDto>>('/categories', data);

export const updateCategorie = (id: string, data: unknown) =>
  axiosInstance.put<ApiResponse<CategorieDto>>(`/categories/${id}`, data);

export const deleteCategorie = (id: string) =>
  axiosInstance.delete(`/categories/${id}`);
