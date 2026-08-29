/**
 * ⚠️ PAGE TEMPORAIRE — STUB AUTH UNIQUEMENT
 *
 * Cette page de login est utilisée UNIQUEMENT pendant la phase de développement
 * avec le StubAuthService local.
 *
 * TODO SSO Migration :
 *   - Supprimer ce fichier entièrement
 *   - Remplacer par une redirection OIDC vers le microservice SSO :
 *     window.location.href = `${SSO_BASE_URL}/authorize
 *       ?response_type=code
 *       &client_id=tims-app
 *       &redirect_uri=${encodeURIComponent(window.location.origin + '/callback')}
 *       &scope=openid profile email roles
 *       &state=${generateState()}`
 *   - Créer CallbackPage.jsx pour traiter le code OIDC retourné
 *
 * Les autres pages métier (Dashboard, Interventions, Profil...)
 * ne seront JAMAIS modifiées lors de cette migration.
 */

import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import {
  Box, TextField, Button, Typography, InputAdornment,
  IconButton, Alert, CircularProgress, Divider, Chip
} from '@mui/material'
import EmailRoundedIcon from '@mui/icons-material/EmailRounded'
import LockRoundedIcon from '@mui/icons-material/LockRounded'
import VisibilityRoundedIcon from '@mui/icons-material/VisibilityRounded'
import VisibilityOffRoundedIcon from '@mui/icons-material/VisibilityOffRounded'
import ElectricBoltRoundedIcon from '@mui/icons-material/ElectricBoltRounded'
import WarningAmberRoundedIcon from '@mui/icons-material/WarningAmberRounded'
import { useAuth } from '../../context/AuthContext'

const DEMO_ACCOUNTS = [
  { role: 'Administrateur Technique', email: 'admin@onee.ma',     password: 'Admin@123',     color: '#0ea5e9' },
  { role: 'Directeur Technique',      email: 'directeur@onee.ma', password: 'Directeur@123', color: '#8b5cf6' },
  { role: 'Chef de Service',          email: 'chef1@onee.ma',     password: 'Chef@123',      color: '#f59e0b' },
  { role: 'Technicien',               email: 'tech01@onee.ma',    password: 'Tech@123',      color: '#10b981' },
]

export default function LoginPage() {
  const { login } = useAuth()
  const navigate  = useNavigate()
  const [showPwd, setShowPwd] = useState(false)
  const [error, setError]     = useState('')
  const [loading, setLoading] = useState(false)

  const { register, handleSubmit, setValue, formState: { errors } } = useForm()

  const onSubmit = async ({ email, password }) => {
    setLoading(true); setError('')
    try { await login(email, password); navigate('/') }
    catch (e) { setError(e.response?.data?.message || 'Identifiants invalides') }
    finally { setLoading(false) }
  }

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', bgcolor: '#f1f5f9' }}>
      {/* Left panel */}
      <Box sx={{ display: { xs: 'none', md: 'flex' }, width: '45%', flexDirection: 'column',
        justifyContent: 'center', px: 8,
        background: 'linear-gradient(135deg, #0f172a 0%, #1e3a5f 60%, #0ea5e9 100%)',
        position: 'relative', overflow: 'hidden' }}>
        <Box sx={{ position:'absolute', top:-100, right:-100, width:350, height:350, borderRadius:'50%', bgcolor:'rgba(14,165,233,0.08)' }} />
        <Box sx={{ position:'absolute', bottom:-80, left:-80, width:280, height:280, borderRadius:'50%', bgcolor:'rgba(255,255,255,0.04)' }} />
        <Box sx={{ position: 'relative', zIndex: 1 }}>
          <Box sx={{ display:'flex', alignItems:'center', gap:1.5, mb:5 }}>
            <Box sx={{ width:44, height:44, borderRadius:'12px', bgcolor:'#0ea5e9', display:'flex', alignItems:'center', justifyContent:'center' }}>
              <ElectricBoltRoundedIcon sx={{ fontSize:24, color:'white' }} />
            </Box>
            <Box>
              <Typography sx={{ color:'white', fontWeight:800, fontSize:'1.1rem', lineHeight:1.1 }}>ONEE TIMS</Typography>
              <Typography sx={{ color:'rgba(255,255,255,0.5)', fontSize:'0.68rem' }}>Technical Management System</Typography>
            </Box>
          </Box>
          <Typography sx={{ color:'white', fontWeight:700, fontSize:'2rem', lineHeight:1.3, mb:2 }}>
            Gestion des<br />Interventions<br />Techniques
          </Typography>
          <Typography sx={{ color:'rgba(255,255,255,0.6)', fontSize:'0.9rem', lineHeight:1.8, maxWidth:340 }}>
            Plateforme professionnelle de suivi et de gestion des interventions de la Direction Technique de l'ONEE.
          </Typography>
          <Box sx={{ mt:5, display:'flex', gap:3 }}>
            {[['200+','Interventions'],['25','Techniciens'],['10','Équipes']].map(([n,l]) => (
              <Box key={l}>
                <Typography sx={{ color:'white', fontWeight:700, fontSize:'1.4rem' }}>{n}</Typography>
                <Typography sx={{ color:'rgba(255,255,255,0.5)', fontSize:'0.72rem' }}>{l}</Typography>
              </Box>
            ))}
          </Box>

          {/* SSO Ready badge */}
          <Box sx={{ mt:5, p:1.5, bgcolor:'rgba(14,165,233,0.12)', borderRadius:'8px',
            border:'1px solid rgba(14,165,233,0.2)', display:'inline-flex', alignItems:'center', gap:1 }}>
            <Box sx={{ width:8, height:8, borderRadius:'50%', bgcolor:'#0ea5e9' }} />
            <Typography sx={{ color:'rgba(255,255,255,0.7)', fontSize:'0.72rem' }}>
              SSO-Ready Architecture — Migration sans modification métier
            </Typography>
          </Box>
        </Box>
      </Box>

      {/* Right panel */}
      <Box sx={{ flex:1, display:'flex', alignItems:'center', justifyContent:'center', px:{ xs:3, md:6 } }}>
        <Box sx={{ width:'100%', maxWidth:400 }}>
          {/* STUB warning banner */}
          <Box sx={{ mb:3, p:1.5, bgcolor:'#fffbeb', border:'1px solid #fde68a',
            borderRadius:'8px', display:'flex', alignItems:'flex-start', gap:1 }}>
            <WarningAmberRoundedIcon sx={{ fontSize:16, color:'#d97706', mt:0.25, flexShrink:0 }} />
            <Typography sx={{ fontSize:'0.72rem', color:'#92400e' }}>
              <strong>Mode Stub temporaire</strong> — Authentification locale.<br />
              Sera remplacé par le microservice SSO.
            </Typography>
          </Box>

          <Typography sx={{ fontWeight:700, fontSize:'1.5rem', color:'#0f172a', mb:0.75 }}>Connexion</Typography>
          <Typography sx={{ color:'#64748b', fontSize:'0.85rem', mb:3 }}>Connectez-vous à votre espace TIMS</Typography>

          {error && <Alert severity="error" sx={{ mb:2, borderRadius:'8px', fontSize:'0.82rem' }}>{error}</Alert>}

          <form onSubmit={handleSubmit(onSubmit)}>
            <TextField label="Adresse email" fullWidth sx={{ mb:2 }} autoComplete="email" autoFocus
              InputProps={{ startAdornment:<InputAdornment position="start"><EmailRoundedIcon sx={{ fontSize:18, color:'#94a3b8' }} /></InputAdornment> }}
              {...register('email', { required:'Email requis', pattern:{ value:/^\S+@\S+\.\S+$/, message:'Email invalide' } })}
              error={!!errors.email} helperText={errors.email?.message} />
            <TextField label="Mot de passe" type={showPwd?'text':'password'} fullWidth sx={{ mb:2.5 }}
              InputProps={{
                startAdornment:<InputAdornment position="start"><LockRoundedIcon sx={{ fontSize:18, color:'#94a3b8' }} /></InputAdornment>,
                endAdornment:<InputAdornment position="end"><IconButton size="small" onClick={()=>setShowPwd(s=>!s)} edge="end">
                  {showPwd?<VisibilityOffRoundedIcon sx={{ fontSize:18 }} />:<VisibilityRoundedIcon sx={{ fontSize:18 }} />}
                </IconButton></InputAdornment>
              }}
              {...register('password', { required:'Mot de passe requis' })}
              error={!!errors.password} helperText={errors.password?.message} />
            <Button type="submit" fullWidth variant="contained" size="large" disabled={loading}
              sx={{ py:1.25, bgcolor:'#1e3a5f', '&:hover':{ bgcolor:'#152c47' }, fontSize:'0.9rem', fontWeight:600 }}>
              {loading ? <CircularProgress size={22} color="inherit" /> : 'Se connecter'}
            </Button>
          </form>

          <Divider sx={{ my:3 }}>
            <Typography sx={{ fontSize:'0.72rem', color:'#94a3b8', px:1 }}>Comptes de démonstration</Typography>
          </Divider>

          <Box sx={{ display:'flex', flexDirection:'column', gap:1 }}>
            {DEMO_ACCOUNTS.map(acc => (
              <Box key={acc.email} onClick={() => { setValue('email', acc.email); setValue('password', acc.password) }}
                sx={{ display:'flex', alignItems:'center', gap:1.5, p:1.25, borderRadius:'8px',
                  border:'1px solid #e2e8f0', cursor:'pointer', bgcolor:'white', transition:'all .15s',
                  '&:hover':{ borderColor:acc.color, bgcolor:`${acc.color}08` } }}>
                <Box sx={{ width:8, height:8, borderRadius:'50%', bgcolor:acc.color, flexShrink:0 }} />
                <Box flex={1} minWidth={0}>
                  <Typography sx={{ fontSize:'0.72rem', fontWeight:600, color:'#334155' }}>{acc.role}</Typography>
                  <Typography sx={{ fontSize:'0.68rem', color:'#94a3b8' }} noWrap>{acc.email}</Typography>
                </Box>
                <Typography sx={{ fontSize:'0.65rem', color:'#94a3b8', flexShrink:0 }}>cliquer</Typography>
              </Box>
            ))}
          </Box>
        </Box>
      </Box>
    </Box>
  )
}
