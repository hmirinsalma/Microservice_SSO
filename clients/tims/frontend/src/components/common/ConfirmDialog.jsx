import React from 'react'
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Typography } from '@mui/material'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'

export default function ConfirmDialog({ open, title, message, onConfirm, onCancel, loading }) {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <WarningAmberIcon color="warning" />
        {title || 'Confirmation'}
      </DialogTitle>
      <DialogContent>
        <Typography>{message || 'Êtes-vous sûr de vouloir effectuer cette action ?'}</Typography>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={onCancel} disabled={loading}>Annuler</Button>
        <Button onClick={onConfirm} variant="contained" color="error" disabled={loading}>
          {loading ? 'Traitement...' : 'Confirmer'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
