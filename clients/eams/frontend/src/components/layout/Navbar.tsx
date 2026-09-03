import React, { useEffect, useState } from 'react';
import { Box, Typography, Avatar, Badge, IconButton, Menu, MenuItem, Divider, InputBase, Tooltip } from '@mui/material';
import { IconBell, IconSearch, IconLogout, IconUser, IconChevronDown, IconSettings } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { getUnreadCount } from '../../api/notificationsApi';

const roleLabels: Record<string, string> = {
  Admin_Patrimoine: 'Administrateur Patrimoine',
  Directeur: 'Directeur',
  Chef_de_Service: 'Chef de Service',
  Technicien: 'Technicien Maintenance',
};

const roleColors: Record<string, string> = {
  Admin_Patrimoine: 'linear-gradient(135deg,#0066CC,#004999)',
  Directeur: 'linear-gradient(135deg,#7B2FBE,#5A1F8C)',
  Chef_de_Service: 'linear-gradient(135deg,#00A86B,#007A4D)',
  Technicien: 'linear-gradient(135deg,#ED8936,#C05621)',
};

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [unread, setUnread] = useState(0);

  useEffect(() => {
    getUnreadCount().then(r => setUnread(r.data.data.count)).catch(() => {});
    const interval = setInterval(() => getUnreadCount().then(r => setUnread(r.data.data.count)).catch(() => {}), 60000);
    return () => clearInterval(interval);
  }, []);

  const handleLogout = async () => { await logout(); navigate('/login'); };
  const initials = user && user.prenom && user.nom 
    ? `${user.prenom[0]}${user.nom[0]}`.toUpperCase() 
    : 'U';

  return (
    <Box sx={{
      height: 64, display: 'flex', alignItems: 'center', px: 3, gap: 2,
      background: '#fff', borderBottom: '1px solid #E2E8F0',
      position: 'sticky', top: 0, zIndex: 99,
      boxShadow: '0 1px 3px rgba(0,0,0,0.04)',
    }}>
      {/* Search */}
      <Box sx={{
        flex: 1, maxWidth: 400,
        display: 'flex', alignItems: 'center', gap: 1,
        px: 1.5, py: 0.8, borderRadius: '10px',
        bgcolor: '#F7FAFC', border: '1px solid #E2E8F0',
        '&:focus-within': { border: '1px solid #0066CC', bgcolor: '#fff', boxShadow: '0 0 0 3px rgba(0,102,204,0.08)' },
        transition: 'all 0.2s',
      }}>
        <IconSearch size={16} color="#A0AEC0" />
        <InputBase placeholder="Rechercher..." sx={{ flex: 1, fontSize: '0.875rem', '& input': { p: 0 } }} />
      </Box>

      <Box sx={{ flex: 1 }} />

      {/* Notifications */}
      <Tooltip title="Notifications">
        <IconButton onClick={() => navigate('/notifications')} sx={{ position: 'relative', color: '#4A5568', '&:hover': { bgcolor: '#F7FAFC' } }}>
          <Badge badgeContent={unread} color="error" max={99}
            sx={{ '& .MuiBadge-badge': { fontSize: '0.65rem', minWidth: 18, height: 18, borderRadius: '9px' } }}>
            <IconBell size={20} />
          </Badge>
        </IconButton>
      </Tooltip>

      {/* User Menu */}
      <Box
        onClick={(e) => setAnchorEl(e.currentTarget)}
        sx={{
          display: 'flex', alignItems: 'center', gap: 1.5, cursor: 'pointer',
          px: 1.5, py: 0.8, borderRadius: '10px',
          '&:hover': { bgcolor: '#F7FAFC' }, transition: 'background 0.15s',
        }}
      >
        <Avatar sx={{
          width: 34, height: 34, fontSize: '0.8rem', fontWeight: 700,
          background: roleColors[user?.role || ''] || 'linear-gradient(135deg,#0066CC,#004999)',
        }}>{initials}</Avatar>
        <Box sx={{ display: { xs: 'none', md: 'block' } }}>
          <Typography sx={{ fontSize: '0.85rem', fontWeight: 600, lineHeight: 1.2, color: '#1A202C' }}>
            {user?.prenom} {user?.nom}
          </Typography>
          <Typography sx={{ fontSize: '0.72rem', color: '#718096', lineHeight: 1.2 }}>
            {roleLabels[user?.role || ''] || user?.role}
          </Typography>
        </Box>
        <IconChevronDown size={14} color="#A0AEC0" />
      </Box>

      <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={() => setAnchorEl(null)}
        PaperProps={{ sx: { mt: 1, borderRadius: '12px', minWidth: 220, boxShadow: '0 10px 40px rgba(0,0,0,0.12)', border: '1px solid #E2E8F0' } }}>
        <Box sx={{ px: 2, py: 1.5 }}>
          <Typography variant="subtitle2" fontWeight={700}>{user?.prenom} {user?.nom}</Typography>
          <Typography variant="caption" color="text.secondary">{user?.email}</Typography>
        </Box>
        <Divider />
        <MenuItem onClick={() => { navigate('/profil'); setAnchorEl(null); }} sx={{ gap: 1.5, py: 1.2, fontSize: '0.875rem' }}>
          <IconUser size={16} />&nbsp;Mon Profil
        </MenuItem>
        <Divider />
        <MenuItem onClick={handleLogout} sx={{ gap: 1.5, py: 1.2, color: '#E53E3E', fontSize: '0.875rem' }}>
          <IconLogout size={16} />&nbsp;Déconnexion
        </MenuItem>
      </Menu>
    </Box>
  );
}
