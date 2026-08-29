import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Box, Grid, Typography, Avatar, Chip, Skeleton, TextField, InputAdornment, LinearProgress, Tooltip } from '@mui/material'
import SearchRoundedIcon from '@mui/icons-material/SearchRounded'
import BuildRoundedIcon from '@mui/icons-material/BuildRounded'
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded'
import { getUsers } from '../../api/users'
import Breadcrumb from '../../components/common/Breadcrumb'

function TechCard({ tech, loading }) {
  const navigate = useNavigate()
  if (loading) return (
    <Box sx={{ bgcolor:'white', borderRadius:'10px', border:'1px solid #e2e8f0', p:2.5 }}>
      <Skeleton variant="circular" width={52} height={52} sx={{ mx:'auto', mb:1 }} />
      <Skeleton width="70%" sx={{ mx:'auto' }} />
      <Skeleton width="50%" sx={{ mx:'auto', mt:0.5 }} />
    </Box>
  )
  const initials = `${tech.firstName?.[0] || ''}${tech.lastName?.[0] || ''}`
  const isAvailable = tech.isActive
  const colors = ['#0ea5e9','#8b5cf6','#10b981','#f59e0b','#ef4444','#1e3a5f']
  const color = colors[(tech.id || 0) % colors.length]

  return (
    <Box onClick={() => navigate(`/users`)}
      sx={{ bgcolor:'white', borderRadius:'10px', border:'1px solid #e2e8f0', p:2.5,
        cursor:'pointer', transition:'all .2s',
        '&:hover': { boxShadow:'0 8px 24px rgba(0,0,0,0.1)', transform:'translateY(-2px)', borderColor:'#0ea5e9' } }}>
      <Box sx={{ textAlign:'center', mb:2 }}>
        <Avatar src={tech.profilePhotoPath}
          sx={{ width:52, height:52, mx:'auto', mb:1.5, fontSize:'1rem',
            bgcolor: color, border:`3px solid ${color}20` }}>
          {initials}
        </Avatar>
        <Typography sx={{ fontWeight:700, fontSize:'0.9rem', color:'#0f172a' }}>
          {tech.firstName} {tech.lastName}
        </Typography>
        <Typography sx={{ fontSize:'0.72rem', color:'#64748b', mt:0.25 }}>{tech.poste || 'Technicien'}</Typography>
      </Box>

      <Box sx={{ display:'flex', justifyContent:'center', mb:1.5 }}>
        <Chip label={isAvailable ? 'Disponible' : 'Inactif'} size="small"
          sx={{ bgcolor: isAvailable ? '#dcfce7':'#fee2e2', color: isAvailable ? '#15803d':'#b91c1c',
            fontWeight:600, fontSize:'0.68rem', height:20 }} />
      </Box>

      <Box sx={{ display:'flex', justifyContent:'space-between', pt:1.5, borderTop:'1px solid #f1f5f9' }}>
        <Box sx={{ textAlign:'center' }}>
          <Typography sx={{ fontSize:'0.68rem', color:'#94a3b8', textTransform:'uppercase', letterSpacing:'0.05em' }}>Service</Typography>
          <Typography sx={{ fontSize:'0.75rem', fontWeight:600, color:'#334155' }} noWrap>{tech.serviceName || '—'}</Typography>
        </Box>
        <Box sx={{ textAlign:'center' }}>
          <Typography sx={{ fontSize:'0.68rem', color:'#94a3b8', textTransform:'uppercase', letterSpacing:'0.05em' }}>Équipe</Typography>
          <Typography sx={{ fontSize:'0.75rem', fontWeight:600, color:'#334155' }} noWrap>{tech.equipeName || '—'}</Typography>
        </Box>
      </Box>
    </Box>
  )
}

export default function TechniciensPage() {
  const [techs, setTechs] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')

  useEffect(() => {
    getUsers(1, 100).then(r => {
      const all = r.data.data.items.filter(u => u.roles?.includes('Technicien'))
      setTechs(all)
    }).finally(() => setLoading(false))
  }, [])

  const filtered = techs.filter(t =>
    `${t.firstName} ${t.lastName} ${t.serviceName} ${t.equipeName}`.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
        <Box>
          <Typography variant="h5">Techniciens</Typography>
          <Typography variant="body2" color="text.secondary" mt={0.5}>{techs.length} techniciens enregistrés</Typography>
        </Box>
        <TextField size="small" placeholder="Rechercher un technicien…"
          value={search} onChange={e => setSearch(e.target.value)}
          InputProps={{ startAdornment: <InputAdornment position="start"><SearchRoundedIcon sx={{ fontSize:16, color:'#94a3b8' }} /></InputAdornment> }}
          sx={{ width:260 }} />
      </Box>
      <Grid container spacing={2}>
        {loading ? Array.from({length:12}).map((_, i) => (
          <Grid item xs={12} sm={6} md={4} lg={3} key={i}><TechCard loading /></Grid>
        )) : filtered.map(t => (
          <Grid item xs={12} sm={6} md={4} lg={3} key={t.id}><TechCard tech={t} /></Grid>
        ))}
        {!loading && filtered.length === 0 && (
          <Grid item xs={12}>
            <Box sx={{ textAlign:'center', py:8, color:'#94a3b8' }}>
              <Typography>Aucun technicien trouvé</Typography>
            </Box>
          </Grid>
        )}
      </Grid>
    </Box>
  )
}
