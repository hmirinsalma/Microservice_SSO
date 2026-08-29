import api from './axios'

export const getInterventions  = (params) => api.get('/interventions', { params })
export const getIntervention   = (id)     => api.get(`/interventions/${id}`)
export const createIntervention= (data)   => api.post('/interventions', data)
export const updateIntervention= (id, d)  => api.put(`/interventions/${id}`, d)
export const deleteIntervention= (id)     => api.delete(`/interventions/${id}`)
export const changeStatus      = (id, d)  => api.patch(`/interventions/${id}/status`, d)
export const changePriority    = (id, d)  => api.patch(`/interventions/${id}/priority`, d)
export const assignTech        = (id, d)  => api.patch(`/interventions/${id}/assign`, d)
export const addComment        = (id, d)  => api.post(`/interventions/${id}/comments`, d)
export const updateCompteRendu = (id, d)  => api.patch(`/interventions/${id}/compte-rendu`, d)
export const getHistory        = (id)     => api.get(`/interventions/${id}/history`)
export const addAttachment     = (id, f)  => {
  const fd = new FormData(); fd.append('file', f)
  return api.post(`/interventions/${id}/attachments`, fd, { headers: { 'Content-Type': 'multipart/form-data' } })
}
export const deleteAttachment  = (aid)    => api.delete(`/interventions/attachments/${aid}`)
export const downloadAttachment= (aid)    => api.get(`/interventions/attachments/${aid}/download`, { responseType: 'blob' })
