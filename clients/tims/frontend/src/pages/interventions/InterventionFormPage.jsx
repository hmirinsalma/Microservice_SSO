import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm, Controller } from 'react-hook-form'
import {
  Box, Grid, Card, CardContent, Typography, TextField, Button,
  FormControl, InputLabel, Select, MenuItem, CircularProgress
} from '@mui/material'
import SaveIcon from '@mui/icons-material/Save'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import { createIntervention, updateIntervention, getIntervention } from '../../api/interventions'
import { getServices, getEquipes, getTechsByService } from '../../api/users'
import { useSnackbar } from '../../context/SnackbarContext'
import { useAuth } from '../../context/AuthContext'
import Breadcrumb from '../../components/common/Breadcrumb'
import PageLoader from '../../components/common/PageLoader'

export default function InterventionFormPage() {
  const { id } = useParams()
  const isEdit  = !!id
  const navigate = useNavigate()
  const { notify } = useSnackbar()
  const { user } = useAuth()

  const [loading, setLoading]   = useState(isEdit)
  const [saving, setSaving]     = useState(false)
  const [services, setServices] = useState([])
  const [equipes, setEquipes]   = useState([])
  const [techs, setTechs]       = useState([])

  const { register, handleSubmit, control, watch, setValue, formState: { errors } } = useForm({
    defaultValues: { priority: 'Normale', status: 'Nouvelle' }
  })

  const watchService = watch('serviceId')

  useEffect(() => {
    getServices().then(r => setServices(r.data.data))
    getEquipes().then(r => setEquipes(r.data.data))
    if (isEdit) {
      getIntervention(id).then(r => {
        const d = r.data.data
        setValue('objet', d.objet)
        setValue('description', d.description)
        setValue('typeIntervention', d.typeIntervention)
        setValue('categorie', d.categorie)
        setValue('localisation', d.localisation)
        setValue('equipement', d.equipement)
        setValue('datePrevue', d.datePrevue?.slice(0,10))
        setValue('priority', d.priority)
        setValue('serviceId', d.service?.id)
        setValue('equipeId', d.equipe?.id)
        setValue('technicienId', d.technicien?.id)
        setValue('responsableId', d.responsable?.id)
      }).finally(() => setLoading(false))
    }
  }, [id])

  useEffect(() => {
    if (watchService) getTechsByService(watchService).then(r => setTechs(r.data.data))
  }, [watchService])

  const onSubmit = async (data) => {
    setSaving(true)
    try {
      const payload = { ...data, serviceId: data.serviceId || user?.serviceId }
      if (isEdit) await updateIntervention(id, payload)
      else        await createIntervention(payload)
      notify(isEdit ? 'Intervention mise à jour' : 'Intervention créée')
      navigate('/interventions')
    } catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  if (loading) return <PageLoader />

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" alignItems="center" gap={2} mb={3}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/interventions')}>Retour</Button>
        <Typography variant="h5" fontWeight={700}>
          {isEdit ? 'Modifier l\'intervention' : 'Nouvelle intervention'}
        </Typography>
      </Box>

      <Card>
        <CardContent sx={{ p: 3 }}>
          <form onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <TextField label="Objet *" fullWidth {...register('objet', { required: 'Requis' })}
                  error={!!errors.objet} helperText={errors.objet?.message} />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField label="Type d'intervention *" fullWidth {...register('typeIntervention', { required: 'Requis' })}
                  error={!!errors.typeIntervention} helperText={errors.typeIntervention?.message} />
              </Grid>
              <Grid item xs={12}>
                <TextField label="Description *" fullWidth multiline rows={3}
                  {...register('description', { required: 'Requis' })}
                  error={!!errors.description} helperText={errors.description?.message} />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField label="Catégorie *" fullWidth {...register('categorie', { required: 'Requis' })}
                  error={!!errors.categorie} helperText={errors.categorie?.message} />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField label="Localisation *" fullWidth {...register('localisation', { required: 'Requis' })}
                  error={!!errors.localisation} helperText={errors.localisation?.message} />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField label="Équipement concerné *" fullWidth {...register('equipement', { required: 'Requis' })}
                  error={!!errors.equipement} helperText={errors.equipement?.message} />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField label="Date prévue *" type="date" fullWidth InputLabelProps={{ shrink: true }}
                  {...register('datePrevue', { required: 'Requis' })}
                  error={!!errors.datePrevue} helperText={errors.datePrevue?.message} />
              </Grid>
              <Grid item xs={12} md={4}>
                <Controller name="priority" control={control} render={({ field }) => (
                  <FormControl fullWidth>
                    <InputLabel>Priorité</InputLabel>
                    <Select {...field} label="Priorité">
                      {['Faible','Normale','Urgente','Critique'].map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={12} md={4}>
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
              <Grid item xs={12} md={4}>
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
              <Grid item xs={12} md={4}>
                <Controller name="technicienId" control={control} render={({ field }) => (
                  <FormControl fullWidth>
                    <InputLabel>Technicien</InputLabel>
                    <Select {...field} label="Technicien" value={field.value || ''}>
                      <MenuItem value="">— Non affecté —</MenuItem>
                      {techs.map(t => <MenuItem key={t.id} value={t.id}>{t.firstName} {t.lastName}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>

              <Grid item xs={12}>
                <Box display="flex" gap={2} justifyContent="flex-end">
                  <Button onClick={() => navigate('/interventions')}>Annuler</Button>
                  <Button type="submit" variant="contained" startIcon={saving ? <CircularProgress size={18} color="inherit" /> : <SaveIcon />} disabled={saving}>
                    {isEdit ? 'Enregistrer' : 'Créer'}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
    </Box>
  )
}
