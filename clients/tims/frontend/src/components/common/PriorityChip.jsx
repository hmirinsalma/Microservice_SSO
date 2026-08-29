import React from 'react'
import { Chip } from '@mui/material'
import { PRIORITY_CONFIG } from '../../utils/constants'

export default function PriorityChip({ priority, size = 'small' }) {
  const cfg = PRIORITY_CONFIG[priority] || { label: priority, bgColor: '#eee', color: '#333' }
  return (
    <Chip
      label={cfg.label}
      size={size}
      sx={{ bgcolor: cfg.bgColor, color: cfg.color, fontWeight: 600, borderRadius: '6px' }}
    />
  )
}
