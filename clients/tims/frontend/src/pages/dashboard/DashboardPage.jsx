import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Grid, Box, Typography, Skeleton, Chip } from '@mui/material'
import BuildRoundedIcon from '@mui/icons-material/BuildRounded'
import PlayArrowRoundedIcon from '@mui/icons-material/PlayArrowRounded'
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded'
import PriorityHighRoundedIcon from '@mui/icons-material/PriorityHighRounded'
import ErrorRoundedIcon from '@mui/icons-material/ErrorRounded'
import PauseRoundedIcon from '@mui/icons-material/PauseRounded'
import FiberNewRoundedIcon from '@mui/icons-material/FiberNewRounded'
import EngineeringRoundedIcon from '@mui/icons-material/EngineeringRounded'
import {
  AreaChart, Area, BarChart, Bar, PieChart, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend
} from 'recharts'
import { getDashboard } from '../../api/dashboard'
import { useAuth } from '../../context/AuthContext'
import { ROLES } from '../../utils/constants'
import Breadcrumb from '../../components/common/Breadcrumb'

const STAT_COLORS = ['#0ea5e9','#f59e0b','#10b981','#8b5cf6','#ef4444','#64748b']

function KpiCard({ title, value, icon, color, subtitle, trend, loading }) {
  return (
    <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0',
      p: 2.5, display: 'flex', alignItems: 'flex-start', gap: 2,
      transition: 'box-shadow .2s', '&:hover': { boxShadow: '0 4px 20px rgba(0,0,0,0.08)' } }}>
      <Box sx={{ width: 44, height: 44, borderRadius: '10px', flexShrink: 0,
        bgcolor: `${color}15`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        {React.cloneElement(icon, { sx: { fontSize: 22, color } })}
      </Box>
      <Box flex={1} minWidth={0}>
        <Typography sx={{ fontSize: '0.72rem', color: '#64748b', fontWeight: 500, textTransform: 'uppercase', letterSpacing: '0.05em' }}>
          {title}
        </Typography>
        {loading ? <Skeleton width={60} height={36} /> : (
          <Typography sx={{ fontSize: '1.8rem', fontWeight: 700, color: '#0f172a', lineHeight: 1.2 }}>
            {value ?? '—'}
          </Typography>
        )}
        {subtitle && !loading && (
          <Typography sx={{ fontSize: '0.72rem', color: '#94a3b8', mt: 0.5 }}>{subtitle}</Typography>
        )}
      </Box>
    </Box>
  )
}

function ChartCard({ title, subtitle, children, loading }) {
  return (
    <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', p: 2.5 }}>
      <Box mb={2}>
        <Typography variant="subtitle1" fontWeight={700} color="#0f172a">{title}</Typography>
        {subtitle && <Typography variant="caption" color="text.secondary">{subtitle}</Typography>}
      </Box>
      {loading ? <Skeleton variant="rectangular" height={220} sx={{ borderRadius: 2 }} /> : children}
    </Box>
  )
}

const CustomTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <Box sx={{ bgcolor: '#0f172a', color: '#fff', p: 1.5, borderRadius: '8px',
      fontSize: '0.78rem', boxShadow: '0 4px 20px rgba(0,0,0,0.2)' }}>
      <Typography sx={{ fontSize: '0.75rem', color: '#94a3b8', mb: 0.5 }}>{label}</Typography>
      {payload.map((p, i) => (
        <Box key={i} sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          <Box sx={{ width: 8, height: 8, borderRadius: '50%', bgcolor: p.color }} />
          <span>{p.name}: <strong>{p.value}</strong></span>
        </Box>
      ))}
    </Box>
  )
}

export default function DashboardPage() {
  const { role } = useAuth()
  const navigate = useNavigate()
  const [data, setData]       = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    getDashboard().then(r => setData(r.data.data)).catch(() => {}).finally(() => setLoading(false))
  }, [])

  const d = data || {}

  if (role === ROLES.ADMIN || role === ROLES.DIRECTEUR) return (
    <Box>
      <Breadcrumb />
      <Box mb={3}>
        <Typography variant="h5">Tableau de bord</Typography>
        <Typography variant="body2" color="text.secondary" mt={0.5}>
          Vue d'ensemble des interventions techniques — ONEE
        </Typography>
      </Box>

      {/* KPIs row */}
      <Grid container spacing={2} mb={3}>
        {[
          { title:'Total interventions', value:d.totalInterventions, icon:<BuildRoundedIcon />,        color:'#1e3a5f', subtitle:'Toutes périodes' },
          { title:'Nouvelles',           value:d.nouvelles,          icon:<FiberNewRoundedIcon />,      color:'#0ea5e9', subtitle:'En attente' },
          { title:'En cours',            value:d.enCours,            icon:<PlayArrowRoundedIcon />,     color:'#f59e0b', subtitle:'Actives' },
          { title:'Suspendues',          value:d.suspendues,         icon:<PauseRoundedIcon />,         color:'#8b5cf6', subtitle:'En pause' },
          { title:'Terminées',           value:d.terminees,          icon:<CheckCircleRoundedIcon />,   color:'#10b981', subtitle:'Clôturées' },
          { title:'Urgentes',            value:d.urgentes,           icon:<PriorityHighRoundedIcon />,  color:'#f59e0b', subtitle:'Haute priorité' },
          { title:'Critiques',           value:d.critiques,          icon:<ErrorRoundedIcon />,         color:'#ef4444', subtitle:'Priorité max' },
        ].map((k, i) => (
          <Grid item xs={6} sm={4} md={12/7*1} key={i} sx={{ minWidth: 140 }}>
            <KpiCard {...k} loading={loading} />
          </Grid>
        ))}
      </Grid>

      {/* Charts row 1 */}
      <Grid container spacing={2} mb={2}>
        <Grid item xs={12} md={8}>
          <ChartCard title="Interventions par statut" subtitle="Répartition actuelle" loading={loading}>
            <ResponsiveContainer width="100%" height={220}>
              <BarChart data={d.byStatus || []} margin={{ left: -20, bottom: 0 }} barSize={28}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
                <XAxis dataKey="label" tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} />
                <YAxis allowDecimals={false} tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} />
                <Tooltip content={<CustomTooltip />} cursor={{ fill: '#f8fafc' }} />
                <Bar dataKey="count" name="Interventions" radius={[4,4,0,0]}>
                  {(d.byStatus || []).map((_, i) => <Cell key={i} fill={STAT_COLORS[i % STAT_COLORS.length]} />)}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </ChartCard>
        </Grid>
        <Grid item xs={12} md={4}>
          <ChartCard title="Par priorité" subtitle="Distribution" loading={loading}>
            <ResponsiveContainer width="100%" height={220}>
              <PieChart>
                <Pie data={d.byPriority || []} dataKey="count" nameKey="label"
                  cx="50%" cy="50%" innerRadius={55} outerRadius={85}
                  paddingAngle={3}>
                  {(d.byPriority || []).map((_, i) => (
                    <Cell key={i} fill={['#94a3b8','#0ea5e9','#f59e0b','#ef4444'][i % 4]} />
                  ))}
                </Pie>
                <Tooltip content={<CustomTooltip />} />
                <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: '0.75rem' }} />
              </PieChart>
            </ResponsiveContainer>
          </ChartCard>
        </Grid>
      </Grid>

      {/* Charts row 2 */}
      <Grid container spacing={2}>
        <Grid item xs={12} md={6}>
          <ChartCard title="Par équipe" subtitle="Volume d'interventions" loading={loading}>
            <ResponsiveContainer width="100%" height={200}>
              <BarChart data={(d.byEquipe || []).slice(0,8)} layout="vertical" margin={{ left: 10 }} barSize={14}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" horizontal={false} />
                <XAxis type="number" allowDecimals={false} tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} />
                <YAxis type="category" dataKey="label" tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} width={110} />
                <Tooltip content={<CustomTooltip />} cursor={{ fill: '#f8fafc' }} />
                <Bar dataKey="count" fill="#0ea5e9" name="Interventions" radius={[0,4,4,0]} />
              </BarChart>
            </ResponsiveContainer>
          </ChartCard>
        </Grid>
        <Grid item xs={12} md={6}>
          <ChartCard title="Par service" subtitle="Répartition par entité" loading={loading}>
            <ResponsiveContainer width="100%" height={200}>
              <PieChart>
                <Pie data={d.byService || []} dataKey="count" nameKey="label"
                  cx="50%" cy="50%" outerRadius={75} paddingAngle={3}>
                  {(d.byService || []).map((_, i) => (
                    <Cell key={i} fill={STAT_COLORS[i % STAT_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip content={<CustomTooltip />} />
                <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: '0.72rem' }} />
              </PieChart>
            </ResponsiveContainer>
          </ChartCard>
        </Grid>
      </Grid>
    </Box>
  )

  if (role === ROLES.CHEF) return (
    <Box>
      <Breadcrumb />
      <Typography variant="h5" mb={3}>Mon Service</Typography>
      <Grid container spacing={2} mb={3}>
        {[
          { title:'Interventions service', value:d.totalServiceInterventions, icon:<BuildRoundedIcon />,       color:'#1e3a5f' },
          { title:'Urgentes/Critiques',    value:d.urgentes,                   icon:<PriorityHighRoundedIcon />,color:'#ef4444' },
          { title:'En attente',            value:d.enAttente,                  icon:<FiberNewRoundedIcon />,    color:'#0ea5e9' },
          { title:'Techniciens dispo',     value:d.techniciensDisponibles,     icon:<EngineeringRoundedIcon />, color:'#10b981', subtitle:`${d.techniciensOccupes || 0} occupé(s)` },
        ].map((k, i) => <Grid item xs={6} md={3} key={i}><KpiCard {...k} loading={loading} /></Grid>)}
      </Grid>
      <ChartCard title="Répartition par statut" loading={loading}>
        <ResponsiveContainer width="100%" height={240}>
          <BarChart data={d.byStatus || []} barSize={32} margin={{ left: -20 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
            <XAxis dataKey="label" tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} />
            <YAxis allowDecimals={false} tick={{ fontSize: 11, fill: '#64748b' }} axisLine={false} tickLine={false} />
            <Tooltip content={<CustomTooltip />} cursor={{ fill: '#f8fafc' }} />
            <Bar dataKey="count" name="Interventions" radius={[4,4,0,0]}>
              {(d.byStatus || []).map((_, i) => <Cell key={i} fill={STAT_COLORS[i % STAT_COLORS.length]} />)}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </ChartCard>
    </Box>
  )

  // Technicien
  return (
    <Box>
      <Breadcrumb />
      <Typography variant="h5" mb={3}>Mes interventions</Typography>
      <Grid container spacing={2} mb={3}>
        {[
          { title:'Total affectées', value:d.totalAffectees, icon:<BuildRoundedIcon />,       color:'#1e3a5f' },
          { title:'En cours',        value:d.enCours,        icon:<PlayArrowRoundedIcon />,    color:'#f59e0b' },
          { title:'Terminées',       value:d.terminees,      icon:<CheckCircleRoundedIcon />,  color:'#10b981' },
          { title:'Urgentes',        value:d.urgentes,       icon:<PriorityHighRoundedIcon />, color:'#ef4444' },
        ].map((k, i) => <Grid item xs={6} md={3} key={i}><KpiCard {...k} loading={loading} /></Grid>)}
      </Grid>
      {(d.prochaines || []).length > 0 && (
        <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', p: 2.5 }}>
          <Typography variant="subtitle1" fontWeight={700} mb={2}>Prochaines interventions</Typography>
          {d.prochaines.map(i => (
            <Box key={i.id} onClick={() => navigate(`/interventions/${i.id}`)}
              sx={{ p: 1.5, mb: 1, bgcolor: '#f8fafc', borderRadius: '8px', cursor: 'pointer',
                display: 'flex', alignItems: 'center', gap: 2,
                border: '1px solid #e2e8f0', '&:hover': { bgcolor: '#f1f5f9' }, transition: 'all .15s' }}>
              <Typography sx={{ fontSize: '0.75rem', fontWeight: 700, color: '#0ea5e9', minWidth: 140 }}>
                {i.numeroIntervention}
              </Typography>
              <Typography sx={{ fontSize: '0.82rem', flex: 1 }} noWrap>{i.objet}</Typography>
              <Typography sx={{ fontSize: '0.72rem', color: '#64748b', flexShrink: 0 }}>
                {new Date(i.datePrevue).toLocaleDateString('fr-MA')}
              </Typography>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  )
}
