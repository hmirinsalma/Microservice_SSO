import React from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import {
  Box, Drawer, List, ListItem, ListItemButton, ListItemIcon,
  ListItemText, Tooltip, Typography, Divider, IconButton, Avatar
} from '@mui/material'
import DashboardRoundedIcon from '@mui/icons-material/DashboardRounded'
import BuildRoundedIcon from '@mui/icons-material/BuildRounded'
import AddCircleOutlineRoundedIcon from '@mui/icons-material/AddCircleOutlineRounded'
import GroupsRoundedIcon from '@mui/icons-material/GroupsRounded'
import EngineeringRoundedIcon from '@mui/icons-material/EngineeringRounded'
import HistoryRoundedIcon from '@mui/icons-material/HistoryRounded'
import NotificationsRoundedIcon from '@mui/icons-material/NotificationsRounded'
import PersonRoundedIcon from '@mui/icons-material/PersonRounded'
import PeopleAltRoundedIcon from '@mui/icons-material/PeopleAltRounded'
import BusinessRoundedIcon from '@mui/icons-material/BusinessRounded'
import LogoutRoundedIcon from '@mui/icons-material/LogoutRounded'
import ChevronLeftRoundedIcon from '@mui/icons-material/ChevronLeftRounded'
import ChevronRightRoundedIcon from '@mui/icons-material/ChevronRightRounded'
import ElectricBoltRoundedIcon from '@mui/icons-material/ElectricBoltRounded'
import { useAuth } from '../../context/AuthContext'
import { ROLES } from '../../utils/constants'

export const DRAWER_WIDTH = 240
export const MINI_WIDTH   = 64

const NAV = [
  { label: 'Tableau de bord', icon: <DashboardRoundedIcon />,        path: '/',              roles: null },
  { label: 'Interventions',   icon: <BuildRoundedIcon />,            path: '/interventions', roles: null },
  { label: 'Nouvelle intervention', icon: <AddCircleOutlineRoundedIcon />, path: '/interventions/new', roles: [ROLES.ADMIN, ROLES.CHEF] },
  { divider: true, label: 'Organisation', roles: [ROLES.ADMIN, ROLES.DIRECTEUR] },
  { label: 'Utilisateurs',    icon: <PeopleAltRoundedIcon />,        path: '/users',         roles: [ROLES.ADMIN] },
  { label: 'Techniciens',     icon: <EngineeringRoundedIcon />,      path: '/techniciens',   roles: [ROLES.ADMIN, ROLES.CHEF, ROLES.DIRECTEUR] },
  { label: 'Équipes & Services', icon: <GroupsRoundedIcon />,        path: '/services',      roles: [ROLES.ADMIN, ROLES.DIRECTEUR] },
  { divider: true, label: 'Suivi', roles: null },
  { label: 'Historique',      icon: <HistoryRoundedIcon />,          path: '/historique',    roles: [ROLES.ADMIN, ROLES.DIRECTEUR, ROLES.CHEF] },
  { label: 'Notifications',   icon: <NotificationsRoundedIcon />,    path: '/notifications', roles: null },
  { divider: true, label: 'Compte', roles: null },
  { label: 'Mon Profil',      icon: <PersonRoundedIcon />,           path: '/profile',       roles: null },
]

export default function Sidebar({ open, onToggle, mobileOpen, onMobileClose }) {
  const { user, role, logout } = useAuth()
  const navigate = useNavigate()
  const { pathname } = useLocation()

  const visible = NAV.filter(n => !n.roles || n.roles.includes(role))
  const collapsed = !open

  const NavItem = ({ item }) => {
    if (item.divider) {
      return collapsed ? (
        <Divider sx={{ my: 1, borderColor: 'rgba(255,255,255,0.08)' }} />
      ) : (
        <Box sx={{ px: 2, pt: 2, pb: 0.5 }}>
          <Typography variant="overline" sx={{ color: 'rgba(255,255,255,0.35)', fontSize: '0.58rem' }}>
            {item.label}
          </Typography>
        </Box>
      )
    }

    const isActive = pathname === item.path || (item.path !== '/' && pathname.startsWith(item.path))

    const btn = (
      <ListItem disablePadding sx={{ mb: 0.25 }}>
        <ListItemButton
          onClick={() => { navigate(item.path); onMobileClose?.() }}
          selected={isActive}
          sx={{
            minHeight: 40, borderRadius: '6px', mx: collapsed ? '8px' : '8px',
            justifyContent: collapsed ? 'center' : 'flex-start',
            px: collapsed ? 1 : 1.5,
            color: isActive ? '#fff' : 'rgba(255,255,255,0.6)',
            bgcolor: isActive ? 'rgba(14,165,233,0.25)' : 'transparent',
            borderLeft: isActive ? '2px solid #0ea5e9' : '2px solid transparent',
            '&:hover': { bgcolor: 'rgba(255,255,255,0.06)', color: '#fff' },
          }}
        >
          <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 0 : 1.5, color: 'inherit', fontSize: 20 }}>
            {React.cloneElement(item.icon, { sx: { fontSize: 19 } })}
          </ListItemIcon>
          {!collapsed && (
            <ListItemText
              primary={item.label}
              primaryTypographyProps={{ fontSize: '0.82rem', fontWeight: isActive ? 600 : 400, noWrap: true }}
            />
          )}
        </ListItemButton>
      </ListItem>
    )

    return collapsed
      ? <Tooltip title={item.label} placement="right" arrow>{btn}</Tooltip>
      : btn
  }

  const content = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column',
      bgcolor: '#0f172a', overflow: 'hidden' }}>

      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', px: collapsed ? 1 : 2,
        py: 1.5, minHeight: 56, borderBottom: '1px solid rgba(255,255,255,0.06)' }}>
        {!collapsed && (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flex: 1, overflow: 'hidden' }}>
            <Box sx={{ width: 32, height: 32, borderRadius: '8px', bgcolor: '#0ea5e9',
              display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <ElectricBoltRoundedIcon sx={{ fontSize: 18, color: '#fff' }} />
            </Box>
            <Box sx={{ overflow: 'hidden' }}>
              <Typography variant="subtitle2" sx={{ color: '#fff', fontWeight: 700, lineHeight: 1.2 }} noWrap>
                ONEE TIMS
              </Typography>
              <Typography sx={{ fontSize: '0.6rem', color: 'rgba(255,255,255,0.4)', lineHeight: 1 }} noWrap>
                Technical Management
              </Typography>
            </Box>
          </Box>
        )}
        {collapsed && (
          <Box sx={{ width: 32, height: 32, borderRadius: '8px', bgcolor: '#0ea5e9',
            display: 'flex', alignItems: 'center', justifyContent: 'center', mx: 'auto' }}>
            <ElectricBoltRoundedIcon sx={{ fontSize: 18, color: '#fff' }} />
          </Box>
        )}
        <IconButton size="small" onClick={onToggle}
          sx={{ color: 'rgba(255,255,255,0.4)', ml: collapsed ? 0 : 1, p: 0.5,
            '&:hover': { color: '#fff', bgcolor: 'rgba(255,255,255,0.08)' } }}>
          {collapsed ? <ChevronRightRoundedIcon sx={{ fontSize: 18 }} /> : <ChevronLeftRoundedIcon sx={{ fontSize: 18 }} />}
        </IconButton>
      </Box>

      {/* User mini card */}
      {!collapsed && (
        <Box sx={{ mx: 1.5, my: 1, p: 1.5, bgcolor: 'rgba(255,255,255,0.04)',
          borderRadius: '8px', border: '1px solid rgba(255,255,255,0.06)' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Avatar sx={{ width: 30, height: 30, fontSize: '0.72rem', bgcolor: '#0ea5e9', flexShrink: 0 }}>
              {user?.firstName?.[0]}{user?.lastName?.[0]}
            </Avatar>
            <Box sx={{ overflow: 'hidden', flex: 1 }}>
              <Typography sx={{ fontSize: '0.75rem', fontWeight: 600, color: '#fff' }} noWrap>
                {user?.firstName} {user?.lastName}
              </Typography>
              <Typography sx={{ fontSize: '0.62rem', color: 'rgba(255,255,255,0.4)' }} noWrap>
                {role?.replace(/_/g, ' ')}
              </Typography>
            </Box>
          </Box>
        </Box>
      )}

      {/* Nav */}
      <Box sx={{ flex: 1, overflowY: 'auto', overflowX: 'hidden', py: 0.5,
        '&::-webkit-scrollbar': { width: 3 },
        '&::-webkit-scrollbar-thumb': { background: 'rgba(255,255,255,0.1)', borderRadius: 2 } }}>
        <List dense disablePadding>
          {visible.map((item, i) => <NavItem key={i} item={item} />)}
        </List>
      </Box>

      {/* Logout */}
      <Box sx={{ p: 1, borderTop: '1px solid rgba(255,255,255,0.06)' }}>
        <Tooltip title={collapsed ? 'Déconnexion' : ''} placement="right">
          <ListItemButton onClick={logout}
            sx={{ borderRadius: '6px', minHeight: 38, color: 'rgba(255,255,255,0.4)',
              justifyContent: collapsed ? 'center' : 'flex-start', px: collapsed ? 1 : 1.5,
              '&:hover': { bgcolor: 'rgba(239,68,68,0.12)', color: '#ef4444' } }}>
            <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 0 : 1.5, color: 'inherit' }}>
              <LogoutRoundedIcon sx={{ fontSize: 19 }} />
            </ListItemIcon>
            {!collapsed && (
              <ListItemText primary="Déconnexion"
                primaryTypographyProps={{ fontSize: '0.82rem', fontWeight: 400 }} />
            )}
          </ListItemButton>
        </Tooltip>
      </Box>
    </Box>
  )

  return (
    <Box component="nav">
      {/* Mobile */}
      <Drawer variant="temporary" open={mobileOpen} onClose={onMobileClose}
        ModalProps={{ keepMounted: true }}
        sx={{ display: { xs: 'block', md: 'none' },
          '& .MuiDrawer-paper': { width: DRAWER_WIDTH, border: 'none' } }}>
        {content}
      </Drawer>
      {/* Desktop */}
      <Drawer variant="permanent"
        sx={{ display: { xs: 'none', md: 'block' },
          width: collapsed ? MINI_WIDTH : DRAWER_WIDTH,
          flexShrink: 0, transition: 'width .2s ease',
          '& .MuiDrawer-paper': {
            width: collapsed ? MINI_WIDTH : DRAWER_WIDTH,
            border: 'none', overflow: 'hidden',
            transition: 'width .2s ease' } }} open>
        {content}
      </Drawer>
    </Box>
  )
}
