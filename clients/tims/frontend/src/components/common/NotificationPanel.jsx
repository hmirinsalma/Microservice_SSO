import React from 'react'
import { Menu, MenuItem, Typography, Box, Chip, Divider, Button, IconButton } from '@mui/material'
import CheckIcon from '@mui/icons-material/Check'
import DoneAllIcon from '@mui/icons-material/DoneAll'
import { formatDistanceToNow } from '../common/dateUtils'

export default function NotificationPanel({ anchorEl, notifications, onClose, onMarkRead, onMarkAll }) {
  return (
    <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={onClose}
      PaperProps={{ sx: { width: 380, maxHeight: 500 } }}>
      <Box sx={{ px: 2, py: 1, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="subtitle1" fontWeight={700}>Notifications</Typography>
        <Button size="small" startIcon={<DoneAllIcon />} onClick={onMarkAll}>
          Tout lire
        </Button>
      </Box>
      <Divider />
      {notifications.length === 0 && (
        <Box sx={{ p: 3, textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">Aucune notification</Typography>
        </Box>
      )}
      {notifications.map(n => (
        <MenuItem key={n.id} sx={{ alignItems: 'flex-start', py: 1.5, px: 2,
          bgcolor: n.isRead ? 'transparent' : 'primary.50',
          '&:hover': { bgcolor: n.isRead ? 'action.hover' : 'primary.100' } }}>
          <Box sx={{ flex: 1 }}>
            <Box display="flex" justifyContent="space-between" alignItems="flex-start">
              <Typography variant="body2" fontWeight={n.isRead ? 400 : 600} sx={{ flex: 1, mr: 1 }}>
                {n.title}
              </Typography>
              {!n.isRead && (
                <IconButton size="small" onClick={() => onMarkRead(n.id)} sx={{ mt: -0.5, mr: -1 }}>
                  <CheckIcon fontSize="small" color="primary" />
                </IconButton>
              )}
            </Box>
            <Typography variant="caption" color="text.secondary" display="block" noWrap>
              {n.message}
            </Typography>
            <Typography variant="caption" color="text.disabled">
              {formatDistanceToNow(n.createdAt)}
            </Typography>
          </Box>
        </MenuItem>
      ))}
    </Menu>
  )
}
