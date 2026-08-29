import axiosInstance from './axiosInstance';

const employesApi = {
  getAll: (params) => axiosInstance.get('/employes', { params }),
  getById: (id) => axiosInstance.get(`/employes/${id}`),
  create: (data) => axiosInstance.post('/employes', data),
  update: (id, data) => axiosInstance.put(`/employes/${id}`, data),
  delete: (id) => axiosInstance.delete(`/employes/${id}`),
};

export default employesApi;
