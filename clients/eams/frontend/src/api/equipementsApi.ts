import axiosInstance from './axiosInstance';
import { ApiResponse, PagedResult, EquipementListDto, EquipementDetailDto, DocumentDto, PhotoDto, HistoriqueEntryDto } from '../types';

export const getEquipements = (params: Record<string, unknown>) =>
  axiosInstance.get<ApiResponse<PagedResult<EquipementListDto>>>('/equipements', { params });

export const getEquipement = (id: string) =>
  axiosInstance.get<ApiResponse<EquipementDetailDto>>(`/equipements/${id}`);

export const createEquipement = (data: unknown) =>
  axiosInstance.post<ApiResponse<EquipementDetailDto>>('/equipements', data);

export const updateEquipement = (id: string, data: unknown) =>
  axiosInstance.put<ApiResponse<EquipementDetailDto>>(`/equipements/${id}`, data);

export const deleteEquipement = (id: string) =>
  axiosInstance.delete(`/equipements/${id}`);

export const updateEtat = (id: string, etat: string) =>
  axiosInstance.patch<ApiResponse<EquipementDetailDto>>(`/equipements/${id}/etat`, { etat });

export const uploadDocument = (id: string, file: File) => {
  const fd = new FormData(); fd.append('file', file);
  return axiosInstance.post<ApiResponse<DocumentDto>>(`/equipements/${id}/documents`, fd, { headers: { 'Content-Type': 'multipart/form-data' } });
};

export const uploadPhoto = (id: string, file: File) => {
  const fd = new FormData(); fd.append('file', file);
  return axiosInstance.post<ApiResponse<PhotoDto>>(`/equipements/${id}/photos`, fd, { headers: { 'Content-Type': 'multipart/form-data' } });
};

export const deleteDocument = (id: string, docId: string) =>
  axiosInstance.delete(`/equipements/${id}/documents/${docId}`);

export const getHistorique = (id: string, page = 1, pageSize = 20) =>
  axiosInstance.get<ApiResponse<PagedResult<HistoriqueEntryDto>>>(`/equipements/${id}/historique`, { params: { page, pageSize } });
