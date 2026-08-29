import axiosInstance from './axiosInstance';
import { ApiResponse, NotificationDto } from '../types';

export const getNotifications = () =>
  axiosInstance.get<ApiResponse<NotificationDto[]>>('/notifications');

export const getUnreadCount = () =>
  axiosInstance.get<ApiResponse<{ count: number }>>('/notifications/unread-count');

export const markAsRead = (id: string) =>
  axiosInstance.patch(`/notifications/${id}/lire`);

export const markAllAsRead = () =>
  axiosInstance.patch('/notifications/lire-tout');
