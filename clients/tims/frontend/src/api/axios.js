import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

// Attach JWT on every request
api.interceptors.request.use(cfg => {
  const token = localStorage.getItem('tims_token')
  if (token) cfg.headers.Authorization = `Bearer ${token}`
  return cfg
})

// Handle 401 globally → redirect to login
api.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401) {
      localStorage.removeItem('tims_token')
      localStorage.removeItem('tims_user')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default api
