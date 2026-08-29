import React, { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  AppBar, Toolbar, IconButton, Box, Badge, Avatar, Typography,
  InputBase, Tooltip, Chip, Menu, MenuItem, ListItemIcon, Divider
} from '@mui/material'
import MenuRoundedIcon from '@mui/icons-material/MenuRounded'
import NotificationsRoundedIcon from '@mui/icons-material/NotificationsRounded'
import SearchRoundedIcon from '@mui/icons-material/SearchRounded'
import PersonRoundedIcon from '@mui/icons-material/PersonRounded'
import LogoutRoundedIcon from '@mui/icons-material/LogoutRounded'
import KeyboardArrowDownRoundedIcon from '@mui/icons-material/KeyboardArrowDownRounded'
import { useAuth } from '../../context/AuthContext'
import { getUnreadCount } from '../../api/dashboard'
import { MINI_WIDTH, DRAWER_WIDTH } from './Sidebar'

const ROLE_COLORS = {
  Administrateur_Technique: '#0ea5e9',
  Directeur_Technique:      '#8b5cf6',
  Chef_de_Service:          '#f59e0b',
  Technicien:               '#10b981',
}
const ROLE_LABELS = {
  Administrateur_Technique: 'Admin Technique',
  Directeur_Technique:      'Directeur',
  Chef_de_Service:          'Chef de Service',
  Technicien:               'Technicien',
}

export default function Navbar({ sidebarOpen, onMenuClick }) {
  const { user, role, logout } = useAuth()
  const navigate = useNavigate()
  const [unread, setUnread] = useState(0)
  const [anchor, setAnchor] = useState(null)
  const width = sidebarOpen ? DRAWER_WIDTH : MINI_WIDTH

  useEffect(() => {
    const fetch = () => getUnreadCount().then(r => setUnread(r.data.data)).catch(() => {})
    fetch()
    const t = setInterval(fetch, 30000)
    return () => clearInterval(t)
  }, [])

  const roleColor = ROLE_COLORS[role] || '#64748b'
  const roleLabel = ROLE_LABELS[role] || role

  return (
    <AppBar position="fixed"
      sx={{ left: { md: width }, width: { md: `calc(100% - ${width}px)` },
        transition: 'left .2s, width .2s',
        bgcolor: '#fff', color: 'text.primary', zIndex: 1200 }}>
      <Toolbar sx={{ minHeight: '56px !important', px: { xs: 2, md: 3 }, gap: 2 }}>
        <IconButton size="small" onClick={onMenuClick}
          sx={{ display: { md: 'none' }, color: 'text.secondary' }}>
          <MenuRoundedIcon />
        </IconButton>

        {/* Search */}
        <Box sx={{ flex: 1, maxWidth: 400, display: { xs: 'none', sm: 'flex' },
          alignItems: 'center', gap: 1, bgcolor: '#f8fafc',
          border: '1px solid #e2e8f0', borderRadius: '8px', px: 1.5, py: 0.5,
          '&:focus-within': { borderColor: '#1e3a5f', bgcolor: '#fff' }, transition: 'all .15s' }}>
          <SearchRoundedIcon sx={{ fontSize: 16, color: '#94a3b8' }} />
          <InputBase placeholder="Rechercher une intervention…"
            sx={{ fontSize: '0.82rem', flex: 1, color: 'text.primary',
              '& input::placeholder': { color: '#94a3b8' } }} />
        </Box>

        <Box flex={1} />

        {/* Notifications */}
        <Tooltip title="Notifications">
          <IconButton size="small" onClick={() => navigate('/notifications')}
            sx={{ bgcolor: '#f8fafc', border: '1px solid #e2e8f0', borderRadius: '8px',
              width: 36, height: 36, '&:hover': { bgcolor: '#f1f5f9' } }}>
            <Badge badgeContent={unread} color="error" max={99}>
              <NotificationsRoundedIcon sx={{ fontSize: 18, color: '#64748b' }} />
            </Badge>
          </IconButton>
        </Tooltip>

        {/* User */}
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1,
          pl: 1, borderLeft: '1px solid #e2e8f0', cursor: 'pointer' }}
          onClick={e => setAnchor(e.currentTarget)}>
          <Avatar src={user?.profilePhotoPath}
            sx={{ width: 32, height: 32, fontSize: '0.72rem',
              bgcolor: roleColor, border: `2px solid ${roleColor}20` }}>
            {user?.firstName?.[0]}{user?.lastName?.[0]}
          </Avatar>
          <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
            <Typography sx={{ fontSize: '0.8rem', fontWeight: 600, lineHeight: 1.2, color: '#0f172a' }}>
              {user?.firstName} {user?.lastName}
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Box sx={{ width: 6, height: 6, borderRadius: '50%', bgcolor: roleColor }} />
              <Typography sx={{ fontSize: '0.68rem', color: '#64748b', lineHeight: 1 }}>
                {roleLabel}
              </Typography>
            </Box>
          </Box>
          <KeyboardArrowDownRoundedIcon sx={{ fontSize: 16, color: '#94a3b8' }} />
        </Box>

        <Menu anchorEl={anchor} open={!!anchor} onClose={() => setAnchor(null)}
          PaperProps={{ sx: { minWidth: 200, mt: 1, borderRadius: '10px' } }}
          transformOrigin={{ horizontal: 'right', vertical: 'top' }}
          anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}>
          <Box sx={{ px: 2, py: 1.5 }}>
            <Typography variant="subtitle2">{user?.firstName} {user?.lastName}</Typography>
            <Typography variant="caption" color="text.secondary">{user?.email}</Typography>
          </Box>
          <Divider />
          <MenuItem onClick={() => { navigate('/profile'); setAnchor(null) }} sx={{ gap: 1.5 }}>
            <ListItemIcon><PersonRoundedIcon fontSize="small" /></ListItemIcon>
            <Typography variant="body2">Mon profil</Typography>
          </MenuItem>
          <Divider />
          <MenuItem onClick={logout} sx={{ gap: 1.5, color: 'error.main' }}>
            <ListItemIcon><LogoutRoundedIcon fontSize="small" color="error" /></ListItemIcon>
            <Typography variant="body2">Déconnexion</Typography>
          </MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  )
}
