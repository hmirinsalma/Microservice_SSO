import api from './axios'

export const getUsers         = (p, ps) => api.get('/users', { params: { page: p, pageSize: ps } })
export const getUser          = (id)    => api.get(`/users/${id}`)
export const createUser       = (data)  => api.post('/users', data)
export const updateUser       = (id, d) => api.put(`/users/${id}`, d)
export const deleteUser       = (id)    => api.delete(`/users/${id}`)
export const updateProfile    = (data)  => api.put('/users/me/profile', data)
export const changePassword   = (data)  => api.post('/users/me/change-password', data)
export const updatePhoto      = (file)  => {
  const fd = new FormData(); fd.append('file', file)
  return api.post('/users/me/photo', fd, { headers: { 'Content-Type': 'multipart/form-data' } })
}
export const getTechsByService = (sid)  => api.get(`/users/technicians/service/${sid}`)

export const getServices   = ()      => api.get('/services')
export const createService = (data)  => api.post('/services', data)
export const updateService = (id, d) => api.put(`/services/${id}`, d)

export const getEquipes         = ()      => api.get('/equipes')
export const getEquipesByService= (sid)   => api.get(`/equipes/service/${sid}`)
export const createEquipe       = (data)  => api.post('/equipes', data)
export const updateEquipe       = (id, d) => api.put(`/equipes/${id}`, d)
