import axiosInstance from './axiosInstance';

const directionsApi = {
  getAll: () => axiosInstance.get('/directions'),
  getById: (id) => axiosInstance.get(`/directions/${id}`),
  create: (data) => axiosInstance.post('/directions', data),
  update: (id, data) => axiosInstance.put(`/directions/${id}`, data),
  delete: (id) => axiosInstance.delete(`/directions/${id}`),
};

export default directionsApi;
