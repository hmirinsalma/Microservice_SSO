import React from 'react'
import { Chip } from '@mui/material'
import { STATUS_CONFIG } from '../../utils/constants'

export default function StatusChip({ status, size = 'small' }) {
  const cfg = STATUS_CONFIG[status] || { label: status, bgColor: '#eee', textColor: '#333' }
  return (
    <Chip
      label={cfg.label}
      size={size}
      sx={{ bgcolor: cfg.bgColor, color: cfg.textColor, fontWeight: 600, borderRadius: '6px' }}
    />
  )
}
