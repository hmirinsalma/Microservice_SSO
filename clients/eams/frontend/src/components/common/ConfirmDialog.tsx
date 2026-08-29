import React from 'react';
import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button, Box, CircularProgress } from '@mui/material';
import { IconAlertTriangle } from '@tabler/icons-react';

interface Props {
  open: boolean; title: string; message: string;
  onConfirm: () => void; onCancel: () => void; loading?: boolean;
}

export default function ConfirmDialog({ open, title, message, onConfirm, onCancel, loading }: Props) {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="xs" fullWidth
      PaperProps={{ sx: { borderRadius: '16px' } }}>
      <DialogTitle sx={{ pb: 1 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <Box sx={{ width: 36, height: 36, borderRadius: '10px', bgcolor: '#FFF5F5', color: '#E53E3E', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
            <IconAlertTriangle size={18} />
          </Box>
          <Box sx={{ fontWeight: 700 }}>{title}</Box>
        </Box>
      </DialogTitle>
      <DialogContent>
        <DialogContentText sx={{ fontSize: '0.875rem', color: '#4A5568' }}>{message}</DialogContentText>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5, gap: 1 }}>
        <Button onClick={onCancel} disabled={loading} sx={{ color: '#718096', borderColor: '#E2E8F0' }} variant="outlined">
          Annuler
        </Button>
        <Button onClick={onConfirm} color="error" variant="contained" disabled={loading}
          sx={{ minWidth: 110, boxShadow: 'none' }}>
          {loading ? <CircularProgress size={18} color="inherit" /> : 'Supprimer'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
