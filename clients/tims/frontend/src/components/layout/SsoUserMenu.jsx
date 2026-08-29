import React, { useState, useEffect } from 'react';
import {
  IconButton,
  Menu,
  MenuItem,
  Avatar,
  Typography,
  Divider,
  Box,
  Chip
} from '@mui/material';
import {
  AccountCircle,
  Logout,
  Dashboard,
  Settings
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import authService from '../../auth/authService';

const SsoUserMenu = () => {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState(null);
  const [user, setUser] = useState(null);
  const [timsContext, setTimsContext] = useState(null);

  useEffect(() => {
    const loadUser = async () => {
      const profile = await authService.getUserProfile();
      const context = await authService.getTimsContext();
      setUser(profile);
      setTimsContext(context);
    };
    loadUser();
  }, []);

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleProfile = () => {
    navigate('/profile');
    handleClose();
  };

  const handleDashboardSso = () => {
    navigate('/dashboard-sso');
    handleClose();
  };

  const handleLogout = async () => {
    handleClose();
    await authService.logout();
  };

  return (
    <>
      <IconButton
        size="large"
        aria-label="account of current user"
        aria-controls="menu-appbar"
        aria-haspopup="true"
        onClick={handleMenu}
        color="inherit"
      >
        <Avatar sx={{ width: 32, height: 32, bgcolor: 'secondary.main' }}>
          {user?.name?.[0] || user?.email?.[0] || '?'}
        </Avatar>
      </IconButton>
      <Menu
        id="menu-appbar"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        {/* User Info */}
        <Box sx={{ px: 2, py: 1.5, minWidth: 280 }}>
          <Typography variant="subtitle1" fontWeight="bold">
            {user?.name || 'Utilisateur'}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {user?.email || 'email@example.com'}
          </Typography>
          
          {/* Roles */}
          {user?.roles && user.roles.length > 0 && (
            <Box sx={{ mt: 1, display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
              {user.roles.map((role, index) => (
                <Chip
                  key={index}
                  label={role}
                  size="small"
                  color="primary"
                  variant="outlined"
                />
              ))}
            </Box>
          )}

          {/* TIMS Context */}
          {timsContext && (
            <Box sx={{ mt: 1.5, pt: 1.5, borderTop: '1px solid #eee' }}>
              <Typography variant="caption" color="text.secondary" display="block">
                Contexte TIMS
              </Typography>
              {timsContext.userId && (
                <Typography variant="caption" display="block">
                  ID: {timsContext.userId}
                </Typography>
              )}
              {timsContext.serviceId && (
                <Typography variant="caption" display="block">
                  Service: {timsContext.serviceId}
                </Typography>
              )}
              {timsContext.teamId && (
                <Typography variant="caption" display="block">
                  Équipe: {timsContext.teamId}
                </Typography>
              )}
            </Box>
          )}
        </Box>

        <Divider />

        {/* Menu Items */}
        <MenuItem onClick={handleProfile}>
          <AccountCircle sx={{ mr: 2 }} />
          Profil
        </MenuItem>
        <MenuItem onClick={handleDashboardSso}>
          <Dashboard sx={{ mr: 2 }} />
          Test SSO
        </MenuItem>
        <Divider />
        <MenuItem onClick={handleLogout} sx={{ color: 'error.main' }}>
          <Logout sx={{ mr: 2 }} />
          Déconnexion
        </MenuItem>
      </Menu>
    </>
  );
};

export default SsoUserMenu;
