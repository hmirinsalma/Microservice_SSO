import axiosInstance from './axiosInstance';

const congesApi = {
  getAll:             (params)    => axiosInstance.get('/conges', { params }),
  getById:            (id)        => axiosInstance.get(`/conges/${id}`),
  create:             (data)      => axiosInstance.post('/conges', data),
  traiterChef:        (id, data)  => axiosInstance.patch(`/conges/${id}/traiter-chef`, data),
  traiterDirecteur:   (id, data)  => axiosInstance.patch(`/conges/${id}/traiter-directeur`, data),
  annuler:            (id)        => axiosInstance.delete(`/conges/${id}`),
};

export default congesApi;
