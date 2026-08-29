import React, { useEffect, useState, useCallback } from 'react'
import {
  Box, Button, Typography, Avatar, Chip, IconButton, Tooltip,
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, FormControl, InputLabel, Select, MenuItem,
  Grid, CircularProgress
} from '@mui/material'
import { DataGrid } from '@mui/x-data-grid'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import BlockIcon from '@mui/icons-material/Block'
import { useForm, Controller } from 'react-hook-form'
import { getUsers, createUser, updateUser, deleteUser, getServices, getEquipes } from '../../api/users'
import { useSnackbar } from '../../context/SnackbarContext'
import ConfirmDialog from '../../components/common/ConfirmDialog'
import Breadcrumb from '../../components/common/Breadcrumb'

const ROLE_OPTIONS = [
  { id: 1, label: 'Administrateur Technique' },
  { id: 2, label: 'Directeur Technique' },
  { id: 3, label: 'Chef de Service' },
  { id: 4, label: 'Technicien' },
]

export default function UsersPage() {
  const { notify } = useSnackbar()
  const [rows, setRows]         = useState([])
  const [total, setTotal]       = useState(0)
  const [loading, setLoading]   = useState(false)
  const [page, setPage]         = useState(0)
  const [pageSize, setPageSize] = useState(20)
  const [services, setServices] = useState([])
  const [equipes, setEquipes]   = useState([])
  const [dialogOpen, setOpen]   = useState(false)
  const [editUser, setEditUser] = useState(null)
  const [deactivate, setDeactivate] = useState(null)
  const [saving, setSaving]     = useState(false)

  const { register, handleSubmit, control, reset, formState: { errors } } = useForm()

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const r = await getUsers(page + 1, pageSize)
      setRows(r.data.data.items)
      setTotal(r.data.data.totalCount)
    } catch { notify('Erreur de chargement', 'error') }
    finally { setLoading(false) }
  }, [page, pageSize])

  useEffect(() => { load() }, [load])
  useEffect(() => {
    getServices().then(r => setServices(r.data.data))
    getEquipes().then(r => setEquipes(r.data.data))
  }, [])

  const openCreate = () => { setEditUser(null); reset({}); setOpen(true) }
  const openEdit   = (u)  => { setEditUser(u); reset({ roleId: u.roles?.[0] ? ROLE_OPTIONS.find(r => r.label === u.roles[0])?.id : '', serviceId: u.serviceId, equipeId: u.equipeId, isActive: u.isActive }); setOpen(true) }

  const onSubmit = async (data) => {
    setSaving(true)
    try {
      if (editUser) await updateUser(editUser.id, data)
      else          await createUser(data)
      notify(editUser ? 'Utilisateur mis à jour' : 'Utilisateur créé')
      setOpen(false); load()
    } catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleDeactivate = async () => {
    setSaving(true)
    try { await deleteUser(deactivate); notify('Utilisateur désactivé'); setDeactivate(null); load() }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const columns = [
    {
      field: 'fullName', headerName: 'Nom', flex: 1, minWidth: 180,
      renderCell: p => (
        <Box display="flex" alignItems="center" gap={1}>
          <Avatar sx={{ width: 32, height: 32, fontSize: 13, bgcolor: 'primary.main' }} src={p.row.profilePhotoPath}>
            {p.row.firstName?.[0]}{p.row.lastName?.[0]}
          </Avatar>
          <Box>
            <Typography variant="body2" fontWeight={600}>{p.row.firstName} {p.row.lastName}</Typography>
            <Typography variant="caption" color="text.secondary">{p.row.email}</Typography>
          </Box>
        </Box>
      )
    },
    { field: 'poste', headerName: 'Poste', width: 160, valueGetter: (_, row) => row.poste || '—' },
    { field: 'roles', headerName: 'Rôle', width: 180, renderCell: p => (
      <Chip label={p.value?.[0]?.replace(/_/g,' ') || '—'} size="small" color="primary" variant="outlined" />
    )},
    { field: 'serviceName', headerName: 'Service', width: 150, valueGetter: (_, row) => row.serviceName || '—' },
    { field: 'isActive', headerName: 'Statut', width: 100, renderCell: p => (
      <Chip label={p.value ? 'Actif' : 'Inactif'} size="small" color={p.value ? 'success' : 'default'} />
    )},
    {
      field: 'actions', headerName: '', width: 100, sortable: false,
      renderCell: p => (
        <Box>
          <Tooltip title="Modifier"><IconButton size="small" onClick={() => openEdit(p.row)}><EditIcon fontSize="small" color="primary" /></IconButton></Tooltip>
          <Tooltip title="Désactiver"><IconButton size="small" onClick={() => setDeactivate(p.row.id)} disabled={!p.row.isActive}><BlockIcon fontSize="small" color="error" /></IconButton></Tooltip>
        </Box>
      )
    }
  ]

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" fontWeight={700}>Gestion des utilisateurs</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>Nouvel utilisateur</Button>
      </Box>

      <Box sx={{ bgcolor: 'white', borderRadius: 3, boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>
        <DataGrid rows={rows} columns={columns} rowCount={total} loading={loading}
          paginationMode="server"
          paginationModel={{ page, pageSize }}
          onPaginationModelChange={m => { setPage(m.page); setPageSize(m.pageSize) }}
          pageSizeOptions={[10, 20, 50]}
          disableRowSelectionOnClick autoHeight
          sx={{ border: 'none', '& .MuiDataGrid-columnHeaders': { bgcolor: '#F5F7FA' } }}
        />
      </Box>

      {/* Create / Edit Dialog */}
      <Dialog open={dialogOpen} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editUser ? 'Modifier l\'utilisateur' : 'Nouvel utilisateur'}</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              {!editUser && (
                <>
                  <Grid item xs={6}>
                    <TextField label="Prénom *" fullWidth {...register('firstName', { required: 'Requis' })} error={!!errors.firstName} helperText={errors.firstName?.message} />
                  </Grid>
                  <Grid item xs={6}>
                    <TextField label="Nom *" fullWidth {...register('lastName', { required: 'Requis' })} error={!!errors.lastName} helperText={errors.lastName?.message} />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField label="Email *" type="email" fullWidth {...register('email', { required: 'Requis' })} error={!!errors.email} helperText={errors.email?.message} />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField label="Mot de passe *" type="password" fullWidth {...register('password', { required: 'Requis', minLength: { value: 8, message: 'Min 8 caractères' } })} error={!!errors.password} helperText={errors.password?.message} />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField label="Poste" fullWidth {...register('poste')} />
                  </Grid>
                </>
              )}
              <Grid item xs={12}>
                <Controller name="roleId" control={control} rules={{ required: 'Requis' }} render={({ field }) => (
                  <FormControl fullWidth error={!!errors.roleId}>
                    <InputLabel>Rôle *</InputLabel>
                    <Select {...field} label="Rôle *" value={field.value || ''}>
                      {ROLE_OPTIONS.map(r => <MenuItem key={r.id} value={r.id}>{r.label}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6}>
                <Controller name="serviceId" control={control} render={({ field }) => (
                  <FormControl fullWidth>
                    <InputLabel>Service</InputLabel>
                    <Select {...field} label="Service" value={field.value || ''}>
                      <MenuItem value="">— Aucun —</MenuItem>
                      {services.map(s => <MenuItem key={s.id} value={s.id}>{s.name}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6}>
                <Controller name="equipeId" control={control} render={({ field }) => (
                  <FormControl fullWidth>
                    <InputLabel>Équipe</InputLabel>
                    <Select {...field} label="Équipe" value={field.value || ''}>
                      <MenuItem value="">— Aucune —</MenuItem>
                      {equipes.map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              {editUser && (
                <Grid item xs={12}>
                  <Controller name="isActive" control={control} render={({ field }) => (
                    <FormControl fullWidth>
                      <InputLabel>Statut</InputLabel>
                      <Select {...field} label="Statut" value={field.value ?? true}>
                        <MenuItem value={true}>Actif</MenuItem>
                        <MenuItem value={false}>Inactif</MenuItem>
                      </Select>
                    </FormControl>
                  )} />
                </Grid>
              )}
            </Grid>
          </DialogContent>
          <DialogActions sx={{ px: 3, pb: 2 }}>
            <Button onClick={() => setOpen(false)}>Annuler</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {saving ? <CircularProgress size={20} color="inherit" /> : (editUser ? 'Enregistrer' : 'Créer')}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      <ConfirmDialog open={!!deactivate} title="Désactiver l'utilisateur"
        message="Cet utilisateur sera désactivé et ne pourra plus se connecter."
        onConfirm={handleDeactivate} onCancel={() => setDeactivate(null)} loading={saving} />
    </Box>
  )
}
