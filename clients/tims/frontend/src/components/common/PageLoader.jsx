import React from 'react'
import { Box, CircularProgress, Typography } from '@mui/material'

export default function PageLoader({ message = 'Chargement...' }) {
  return (
    <Box display="flex" flexDirection="column" alignItems="center" justifyContent="center"
      sx={{ minHeight: 300, gap: 2 }}>
      <CircularProgress size={48} />
      <Typography color="text.secondary">{message}</Typography>
    </Box>
  )
}
