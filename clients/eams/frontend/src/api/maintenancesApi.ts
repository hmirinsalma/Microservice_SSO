import axiosInstance from './axiosInstance';
import { ApiResponse, PagedResult, MaintenanceListDto, MaintenanceDetailDto } from '../types';

export const getMaintenances = (params: Record<string, unknown>) =>
  axiosInstance.get<ApiResponse<PagedResult<MaintenanceListDto>>>('/maintenances', { params });

export const getMaintenance = (id: string) =>
  axiosInstance.get<ApiResponse<MaintenanceDetailDto>>(`/maintenances/${id}`);

export const createMaintenance = (data: unknown) =>
  axiosInstance.post<ApiResponse<MaintenanceDetailDto>>('/maintenances', data);

export const updateMaintenance = (id: string, data: unknown) =>
  axiosInstance.put<ApiResponse<MaintenanceDetailDto>>(`/maintenances/${id}`, data);

export const cloturerMaintenance = (id: string, data: unknown) =>
  axiosInstance.patch<ApiResponse<MaintenanceDetailDto>>(`/maintenances/${id}/cloturer`, data);

export const deleteMaintenance = (id: string) =>
  axiosInstance.delete(`/maintenances/${id}`);
