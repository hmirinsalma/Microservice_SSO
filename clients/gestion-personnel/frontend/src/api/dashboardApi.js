import axiosInstance from './axiosInstance';

const dashboardApi = {
  get: () => axiosInstance.get('/dashboard'),
};

export default dashboardApi;
