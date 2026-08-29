import axiosInstance from './axiosInstance';

const servicesApi = {
  getAll: () => axiosInstance.get('/services'),
  getById: (id) => axiosInstance.get(`/services/${id}`),
  getByDirection: (directionId) => axiosInstance.get(`/services/direction/${directionId}`),
  create: (data) => axiosInstance.post('/services', data),
  update: (id, data) => axiosInstance.put(`/services/${id}`, data),
  delete: (id) => axiosInstance.delete(`/services/${id}`),
};

export default servicesApi;
