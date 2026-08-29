import React, { useState } from 'react'
import {
  Box, Grid, Typography, TextField, Button, Avatar, IconButton,
  Divider, Chip, CircularProgress, Card, CardContent
} from '@mui/material'
import PhotoCameraRoundedIcon from '@mui/icons-material/PhotoCameraRounded'
import SaveRoundedIcon from '@mui/icons-material/SaveRounded'
import LockRoundedIcon from '@mui/icons-material/LockRounded'
import BadgeRoundedIcon from '@mui/icons-material/BadgeRounded'
import EmailRoundedIcon from '@mui/icons-material/EmailRounded'
import PhoneRoundedIcon from '@mui/icons-material/PhoneRounded'
import BusinessRoundedIcon from '@mui/icons-material/BusinessRounded'
import GroupsRoundedIcon from '@mui/icons-material/GroupsRounded'
import WorkRoundedIcon from '@mui/icons-material/WorkRounded'
import { useForm } from 'react-hook-form'
import { updateProfile, changePassword, updatePhoto } from '../../api/users'
import { useAuth } from '../../context/AuthContext'
import { useSnackbar } from '../../context/SnackbarContext'
import Breadcrumb from '../../components/common/Breadcrumb'

const ROLE_COLORS = {
  Administrateur_Technique: '#0ea5e9',
  Directeur_Technique:      '#8b5cf6',
  Chef_de_Service:          '#f59e0b',
  Technicien:               '#10b981',
}
const ROLE_LABELS = {
  Administrateur_Technique: 'Administrateur Technique',
  Directeur_Technique:      'Directeur Technique',
  Chef_de_Service:          'Chef de Service',
  Technicien:               'Technicien',
}

function InfoRow({ icon, label, value }) {
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, py: 1.5,
      borderBottom: '1px solid #f1f5f9', '&:last-child': { borderBottom: 'none' } }}>
      <Box sx={{ color: '#94a3b8', display: 'flex' }}>{icon}</Box>
      <Box flex={1}>
        <Typography sx={{ fontSize: '0.68rem', color: '#94a3b8', textTransform: 'uppercase',
          letterSpacing: '0.05em', mb: 0.25 }}>{label}</Typography>
        <Typography sx={{ fontSize: '0.85rem', fontWeight: 500, color: '#0f172a' }}>
          {value || <span style={{ color: '#94a3b8', fontStyle: 'italic' }}>Non renseigné</span>}
        </Typography>
      </Box>
    </Box>
  )
}

export default function ProfilePage() {
  const { user } = useAuth()
  const { notify } = useSnackbar()
  const [saving, setSaving]     = useState(false)
  const [savingPwd, setSavingPwd] = useState(false)

  const { register: rP, handleSubmit: hP, formState: { errors: eP } } = useForm({
    defaultValues: { phone: user?.phone || '' }
  })
  const { register: rPwd, handleSubmit: hPwd, reset: resetPwd, formState: { errors: ePwd }, watch } = useForm()
  const newPwd = watch('newPassword')

  const onProfile = async (data) => {
    setSaving(true)
    try { await updateProfile(data); notify('Profil mis à jour') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const onPassword = async (data) => {
    setSavingPwd(true)
    try { await changePassword(data); notify('Mot de passe modifié'); resetPwd() }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSavingPwd(false) }
  }

  const onPhoto = async (e) => {
    const file = e.target.files?.[0]; if (!file) return
    try { await updatePhoto(file); notify('Photo mise à jour') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
  }

  const roleColor = ROLE_COLORS[user?.roles?.[0]] || '#64748b'
  const roleLabel = ROLE_LABELS[user?.roles?.[0]] || user?.roles?.[0]

  return (
    <Box>
      <Breadcrumb />
      <Typography variant="h5" mb={3}>Mon Profil</Typography>

      <Grid container spacing={3}>
        {/* Left — identity card */}
        <Grid item xs={12} md={4}>
          <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', overflow: 'hidden' }}>
            {/* Header band */}
            <Box sx={{ height: 80, background: `linear-gradient(135deg, #1e3a5f 0%, ${roleColor} 100%)` }} />

            <Box sx={{ px: 3, pb: 3, textAlign: 'center', mt: '-40px' }}>
              {/* Avatar with edit */}
              <Box sx={{ position: 'relative', display: 'inline-block' }}>
                <Avatar src={user?.profilePhotoPath}
                  sx={{ width: 80, height: 80, fontSize: '1.5rem', mx: 'auto',
                    border: '4px solid white', bgcolor: roleColor,
                    boxShadow: '0 4px 16px rgba(0,0,0,0.12)' }}>
                  {user?.firstName?.[0]}{user?.lastName?.[0]}
                </Avatar>
                <IconButton component="label" size="small"
                  sx={{ position: 'absolute', bottom: 2, right: 2, bgcolor: '#1e3a5f',
                    width: 26, height: 26, '&:hover': { bgcolor: '#152c47' },
                    boxShadow: '0 2px 8px rgba(0,0,0,0.2)' }}>
                  <PhotoCameraRoundedIcon sx={{ fontSize: 14, color: 'white' }} />
                  <input type="file" hidden accept="image/jpeg,image/png,image/webp" onChange={onPhoto} />
                </IconButton>
              </Box>

              <Typography sx={{ fontWeight: 700, fontSize: '1.05rem', mt: 1.5, color: '#0f172a' }}>
                {user?.firstName} {user?.lastName}
              </Typography>
              <Typography sx={{ fontSize: '0.78rem', color: '#64748b', mb: 1.5 }}>
                {user?.poste || 'Poste non défini'}
              </Typography>
              <Chip label={roleLabel} size="small"
                sx={{ bgcolor: `${roleColor}15`, color: roleColor,
                  fontWeight: 700, fontSize: '0.72rem', height: 22 }} />
            </Box>

            <Divider sx={{ borderColor: '#f1f5f9' }} />

            <Box sx={{ px: 3, py: 2 }}>
              <InfoRow icon={<EmailRoundedIcon sx={{ fontSize:18 }} />}   label="Email"   value={user?.email} />
              <InfoRow icon={<PhoneRoundedIcon sx={{ fontSize:18 }} />}   label="Téléphone" value={user?.phone} />
              <InfoRow icon={<BusinessRoundedIcon sx={{ fontSize:18 }} />} label="Service" value={user?.serviceName} />
              <InfoRow icon={<GroupsRoundedIcon sx={{ fontSize:18 }} />}  label="Équipe"  value={user?.equipeName} />
              <InfoRow icon={<WorkRoundedIcon sx={{ fontSize:18 }} />}    label="Poste"   value={user?.poste} />
            </Box>
          </Box>
        </Grid>

        {/* Right — forms */}
        <Grid item xs={12} md={8}>
          {/* Edit profile */}
          <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', p: 3, mb: 3 }}>
            <Box display="flex" alignItems="center" gap={1} mb={2.5}>
              <BadgeRoundedIcon sx={{ fontSize: 20, color: '#1e3a5f' }} />
              <Typography variant="subtitle1" fontWeight={700}>Modifier mes informations</Typography>
            </Box>
            <form onSubmit={hP(onProfile)}>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                  <TextField label="Prénom" fullWidth value={user?.firstName || ''} disabled
                    InputProps={{ sx: { bgcolor: '#f8fafc' } }} />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField label="Nom" fullWidth value={user?.lastName || ''} disabled
                    InputProps={{ sx: { bgcolor: '#f8fafc' } }} />
                </Grid>
                <Grid item xs={12}>
                  <TextField label="Email" fullWidth value={user?.email || ''} disabled
                    InputProps={{ sx: { bgcolor: '#f8fafc' } }} />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField label="Téléphone" fullWidth placeholder="+212 6XX XXX XXX"
                    {...rP('phone')}
                    InputProps={{ startAdornment: <PhoneRoundedIcon sx={{ fontSize:16, color:'#94a3b8', mr:1 }} /> }} />
                </Grid>
              </Grid>
              <Box mt={2.5} display="flex" justifyContent="flex-end">
                <Button type="submit" variant="contained" size="small"
                  startIcon={saving ? <CircularProgress size={14} color="inherit" /> : <SaveRoundedIcon />}
                  disabled={saving}
                  sx={{ bgcolor: '#1e3a5f', '&:hover': { bgcolor: '#152c47' } }}>
                  Enregistrer
                </Button>
              </Box>
            </form>
          </Box>

          {/* Change password — STUB TEMPORAIRE */}
          <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #fde68a', p: 3 }}>
            <Box display="flex" alignItems="center" gap={1} mb={1}>
              <LockRoundedIcon sx={{ fontSize: 20, color: '#d97706' }} />
              <Typography variant="subtitle1" fontWeight={700}>Changer le mot de passe</Typography>
              <Box sx={{ ml: 'auto', px: 1, py: 0.25, bgcolor: '#fffbeb', border: '1px solid #fde68a',
                borderRadius: '4px' }}>
                <Typography sx={{ fontSize: '0.65rem', color: '#92400e', fontWeight: 600 }}>
                  STUB TEMPORAIRE
                </Typography>
              </Box>
            </Box>
            <Typography sx={{ fontSize: '0.72rem', color: '#92400e', mb: 2, p: 1,
              bgcolor: '#fffbeb', borderRadius: '6px' }}>
              ⚠️ Cette fonctionnalité est temporaire. Le changement de mot de passe sera géré
              par le microservice SSO lors de l'intégration.
            </Typography>
            <form onSubmit={hPwd(onPassword)}>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <TextField label="Mot de passe actuel *" type="password" fullWidth
                    {...rPwd('currentPassword', { required: 'Requis' })}
                    error={!!ePwd.currentPassword} helperText={ePwd.currentPassword?.message} />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField label="Nouveau mot de passe *" type="password" fullWidth
                    {...rPwd('newPassword', { required: 'Requis', minLength: { value: 8, message: 'Min 8 caractères' } })}
                    error={!!ePwd.newPassword} helperText={ePwd.newPassword?.message} />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField label="Confirmer *" type="password" fullWidth
                    {...rPwd('confirm', { required: 'Requis', validate: v => v === newPwd || 'Ne correspond pas' })}
                    error={!!ePwd.confirm} helperText={ePwd.confirm?.message} />
                </Grid>
              </Grid>
              <Box mt={2.5} display="flex" justifyContent="flex-end">
                <Button type="submit" variant="contained" size="small" color="warning"
                  startIcon={savingPwd ? <CircularProgress size={14} color="inherit" /> : <LockRoundedIcon />}
                  disabled={savingPwd}>
                  Changer le mot de passe
                </Button>
              </Box>
            </form>
          </Box>
        </Grid>
      </Grid>
    </Box>
  )
}
