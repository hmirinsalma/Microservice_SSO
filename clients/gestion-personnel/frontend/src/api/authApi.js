import axiosInstance from './axiosInstance';

const authApi = {
  login: (credentials) => axiosInstance.post('/auth/login', credentials),
  logout: () => axiosInstance.post('/auth/logout'),
};

export default authApi;
