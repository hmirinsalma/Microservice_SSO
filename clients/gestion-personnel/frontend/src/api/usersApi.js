import axiosInstance from './axiosInstance';

/**
 * API de gestion des comptes utilisateurs.
 * changePassword supprimé — délégué au microservice SSO.
 */
const usersApi = {
  getAll:       ()         => axiosInstance.get('/users'),
  getRoles:     ()         => axiosInstance.get('/users/roles'),
  getById:      (id)       => axiosInstance.get(`/users/${id}`),
  create:       (data)     => axiosInstance.post('/users', data),
  update:       (id, data) => axiosInstance.put(`/users/${id}`, data),
  toggleActive: (id)       => axiosInstance.patch(`/users/${id}/toggle-active`),
  delete:       (id)       => axiosInstance.delete(`/users/${id}`),
  // NOTE : changePassword supprimé — géré par le SSO
};

export default usersApi;
