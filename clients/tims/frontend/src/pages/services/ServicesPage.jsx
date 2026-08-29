import React, { useEffect, useState } from 'react'
import {
  Box, Grid, Card, CardContent, Typography, Button,
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, IconButton, Tooltip, Chip, CircularProgress,
  Table, TableBody, TableCell, TableHead, TableRow, Paper
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import BusinessIcon from '@mui/icons-material/Business'
import GroupsIcon from '@mui/icons-material/Groups'
import { useForm, Controller } from 'react-hook-form'
import {
  getServices, createService, updateService,
  getEquipes, createEquipe, updateEquipe
} from '../../api/users'
import { useSnackbar } from '../../context/SnackbarContext'
import { useAuth } from '../../context/AuthContext'
import { ROLES } from '../../utils/constants'
import Breadcrumb from '../../components/common/Breadcrumb'
import { FormControl, InputLabel, Select, MenuItem } from '@mui/material'

export default function ServicesPage() {
  const { role } = useAuth()
  const { notify } = useSnackbar()
  const isAdmin = role === ROLES.ADMIN

  const [services, setServices]   = useState([])
  const [equipes, setEquipes]     = useState([])
  const [svcDialog, setSvcDialog] = useState(false)
  const [eqDialog, setEqDialog]   = useState(false)
  const [editSvc, setEditSvc]     = useState(null)
  const [editEq, setEditEq]       = useState(null)
  const [saving, setSaving]       = useState(false)

  const svcForm = useForm()
  const eqForm  = useForm()

  const load = async () => {
    const [s, e] = await Promise.all([getServices(), getEquipes()])
    setServices(s.data.data)
    setEquipes(e.data.data)
  }

  useEffect(() => { load() }, [])

  const openSvc = (s = null) => {
    setEditSvc(s)
    svcForm.reset(s ? { name: s.name, description: s.description } : {})
    setSvcDialog(true)
  }

  const openEq = (e = null) => {
    setEditEq(e)
    eqForm.reset(e ? { name: e.name, description: e.description, serviceId: e.serviceId } : {})
    setEqDialog(true)
  }

  const saveSvc = async (data) => {
    setSaving(true)
    try {
      if (editSvc) await updateService(editSvc.id, data)
      else         await createService(data)
      notify(editSvc ? 'Service mis à jour' : 'Service créé')
      setSvcDialog(false); load()
    } catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const saveEq = async (data) => {
    setSaving(true)
    try {
      if (editEq) await updateEquipe(editEq.id, data)
      else        await createEquipe(data)
      notify(editEq ? 'Équipe mise à jour' : 'Équipe créée')
      setEqDialog(false); load()
    } catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  return (
    <Box>
      <Breadcrumb />
      <Typography variant="h5" fontWeight={700} mb={3}>Services & Équipes</Typography>

      <Grid container spacing={3}>
        {/* Services */}
        <Grid item xs={12} md={6}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Box display="flex" alignItems="center" gap={1}>
              <BusinessIcon color="primary" />
              <Typography variant="h6" fontWeight={600}>Services ({services.length})</Typography>
            </Box>
            {isAdmin && <Button size="small" variant="contained" startIcon={<AddIcon />} onClick={() => openSvc()}>Ajouter</Button>}
          </Box>
          <Paper sx={{ borderRadius: 3, overflow: 'hidden', boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>
            <Table size="small">
              <TableHead>
                <TableRow sx={{ bgcolor: '#F5F7FA' }}>
                  <TableCell><strong>Nom</strong></TableCell>
                  <TableCell align="center"><strong>Équipes</strong></TableCell>
                  <TableCell align="center"><strong>Membres</strong></TableCell>
                  {isAdmin && <TableCell></TableCell>}
                </TableRow>
              </TableHead>
              <TableBody>
                {services.map(s => (
                  <TableRow key={s.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight={500}>{s.name}</Typography>
                      {s.description && <Typography variant="caption" color="text.secondary">{s.description}</Typography>}
                    </TableCell>
                    <TableCell align="center"><Chip label={s.equipeCount} size="small" color="primary" variant="outlined" /></TableCell>
                    <TableCell align="center"><Chip label={s.userCount} size="small" /></TableCell>
                    {isAdmin && (
                      <TableCell align="center">
                        <Tooltip title="Modifier">
                          <IconButton size="small" onClick={() => openSvc(s)}><EditIcon fontSize="small" /></IconButton>
                        </Tooltip>
                      </TableCell>
                    )}
                  </TableRow>
                ))}
                {services.length === 0 && (
                  <TableRow><TableCell colSpan={4} align="center"><Typography color="text.secondary" py={2}>Aucun service</Typography></TableCell></TableRow>
                )}
              </TableBody>
            </Table>
          </Paper>
        </Grid>

        {/* Équipes */}
        <Grid item xs={12} md={6}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Box display="flex" alignItems="center" gap={1}>
              <GroupsIcon color="secondary" />
              <Typography variant="h6" fontWeight={600}>Équipes ({equipes.length})</Typography>
            </Box>
            {isAdmin && <Button size="small" variant="contained" color="secondary" startIcon={<AddIcon />} onClick={() => openEq()}>Ajouter</Button>}
          </Box>
          <Paper sx={{ borderRadius: 3, overflow: 'hidden', boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>
            <Table size="small">
              <TableHead>
                <TableRow sx={{ bgcolor: '#F5F7FA' }}>
                  <TableCell><strong>Nom</strong></TableCell>
                  <TableCell><strong>Service</strong></TableCell>
                  <TableCell align="center"><strong>Membres</strong></TableCell>
                  {isAdmin && <TableCell></TableCell>}
                </TableRow>
              </TableHead>
              <TableBody>
                {equipes.map(e => (
                  <TableRow key={e.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight={500}>{e.name}</Typography>
                    </TableCell>
                    <TableCell><Chip label={e.serviceName || '—'} size="small" variant="outlined" /></TableCell>
                    <TableCell align="center"><Chip label={e.memberCount} size="small" /></TableCell>
                    {isAdmin && (
                      <TableCell align="center">
                        <Tooltip title="Modifier">
                          <IconButton size="small" onClick={() => openEq(e)}><EditIcon fontSize="small" /></IconButton>
                        </Tooltip>
                      </TableCell>
                    )}
                  </TableRow>
                ))}
                {equipes.length === 0 && (
                  <TableRow><TableCell colSpan={4} align="center"><Typography color="text.secondary" py={2}>Aucune équipe</Typography></TableCell></TableRow>
                )}
              </TableBody>
            </Table>
          </Paper>
        </Grid>
      </Grid>

      {/* Service dialog */}
      <Dialog open={svcDialog} onClose={() => setSvcDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>{editSvc ? 'Modifier le service' : 'Nouveau service'}</DialogTitle>
        <form onSubmit={svcForm.handleSubmit(saveSvc)}>
          <DialogContent>
            <TextField label="Nom *" fullWidth sx={{ mb: 2 }} {...svcForm.register('name', { required: 'Requis' })} error={!!svcForm.formState.errors.name} helperText={svcForm.formState.errors.name?.message} />
            <TextField label="Description" fullWidth multiline rows={2} {...svcForm.register('description')} />
          </DialogContent>
          <DialogActions sx={{ px: 3, pb: 2 }}>
            <Button onClick={() => setSvcDialog(false)}>Annuler</Button>
            <Button type="submit" variant="contained" disabled={saving}>{saving ? <CircularProgress size={20} color="inherit" /> : 'Enregistrer'}</Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* Equipe dialog */}
      <Dialog open={eqDialog} onClose={() => setEqDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>{editEq ? 'Modifier l\'équipe' : 'Nouvelle équipe'}</DialogTitle>
        <form onSubmit={eqForm.handleSubmit(saveEq)}>
          <DialogContent>
            <TextField label="Nom *" fullWidth sx={{ mb: 2 }} {...eqForm.register('name', { required: 'Requis' })} error={!!eqForm.formState.errors.name} helperText={eqForm.formState.errors.name?.message} />
            <Controller name="serviceId" control={eqForm.control} rules={{ required: 'Requis' }} render={({ field }) => (
              <FormControl fullWidth sx={{ mb: 2 }} error={!!eqForm.formState.errors.serviceId}>
                <InputLabel>Service *</InputLabel>
                <Select {...field} label="Service *" value={field.value || ''}>
                  {services.map(s => <MenuItem key={s.id} value={s.id}>{s.name}</MenuItem>)}
                </Select>
              </FormControl>
            )} />
            <TextField label="Description" fullWidth multiline rows={2} {...eqForm.register('description')} />
          </DialogContent>
          <DialogActions sx={{ px: 3, pb: 2 }}>
            <Button onClick={() => setEqDialog(false)}>Annuler</Button>
            <Button type="submit" variant="contained" disabled={saving}>{saving ? <CircularProgress size={20} color="inherit" /> : 'Enregistrer'}</Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  )
}
