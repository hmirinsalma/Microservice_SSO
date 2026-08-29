import api from './axios'
export const getDashboard       = ()     => api.get('/dashboard')
export const getNotifications   = (p,ps) => api.get('/notifications', { params: { page: p, pageSize: ps } })
export const getUnreadCount     = ()     => api.get('/notifications/unread-count')
export const markRead           = (id)   => api.patch(`/notifications/${id}/read`)
export const markAllRead        = ()     => api.post('/notifications/read-all')
