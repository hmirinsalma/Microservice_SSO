import React from 'react'
import { Card, CardContent, Box, Typography, Avatar } from '@mui/material'

export default function StatCard({ title, value, icon, color = 'primary.main', subtitle }) {
  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Typography variant="body2" color="text.secondary" gutterBottom>{title}</Typography>
            <Typography variant="h4" fontWeight={700} color={color}>{value}</Typography>
            {subtitle && <Typography variant="caption" color="text.secondary">{subtitle}</Typography>}
          </Box>
          <Avatar sx={{ bgcolor: color, width: 52, height: 52, opacity: 0.9 }}>
            {icon}
          </Avatar>
        </Box>
      </CardContent>
    </Card>
  )
}
