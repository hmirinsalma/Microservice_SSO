import React from 'react'
import { Link as RouterLink, useLocation } from 'react-router-dom'
import { Breadcrumbs, Link, Typography, Box } from '@mui/material'
import HomeIcon from '@mui/icons-material/Home'
import NavigateNextIcon from '@mui/icons-material/NavigateNext'

const LABELS = {
  '':             'Accueil',
  interventions:  'Interventions',
  users:          'Utilisateurs',
  teams:          'Équipes',
  services:       'Services',
  profile:        'Mon Profil',
  new:            'Nouvelle',
  edit:           'Modifier',
}

export default function Breadcrumb() {
  const { pathname } = useLocation()
  const parts = pathname.split('/').filter(Boolean)

  return (
    <Box sx={{ mb: 2 }}>
      <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
        <Link component={RouterLink} to="/" color="inherit" underline="hover"
          sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          <HomeIcon sx={{ fontSize: 18 }} />
          Accueil
        </Link>
        {parts.map((part, i) => {
          const path = '/' + parts.slice(0, i + 1).join('/')
          const label = LABELS[part] || decodeURIComponent(part)
          const isLast = i === parts.length - 1
          return isLast
            ? <Typography key={path} color="text.primary" fontWeight={600}>{label}</Typography>
            : <Link key={path} component={RouterLink} to={path} color="inherit" underline="hover">{label}</Link>
        })}
      </Breadcrumbs>
    </Box>
  )
}
