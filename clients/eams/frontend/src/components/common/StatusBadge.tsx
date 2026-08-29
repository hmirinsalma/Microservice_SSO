import React from 'react';
import { Box } from '@mui/material';
import { etatColors } from '../../theme/oneeTheme';
import { etatLabels } from '../../utils/formatters';
import { EquipementEtat } from '../../types';

interface Props { etat: EquipementEtat; size?: 'small' | 'medium' }

export default function StatusBadge({ etat, size = 'small' }: Props) {
  const c = etatColors[etat] || { bg: '#F7FAFC', color: '#4A5568', border: '#CBD5E0' };
  return (
    <Box component="span" sx={{
      display: 'inline-flex', alignItems: 'center', gap: '5px',
      px: size === 'small' ? 1.2 : 1.8,
      py: size === 'small' ? 0.4 : 0.6,
      borderRadius: '20px',
      bgcolor: c.bg, color: c.color,
      border: `1px solid ${c.border}`,
      fontSize: size === 'small' ? '0.72rem' : '0.82rem',
      fontWeight: 600, letterSpacing: '0.01em', whiteSpace: 'nowrap',
    }}>
      <Box component="span" sx={{ width: 6, height: 6, borderRadius: '50%', bgcolor: c.color, flexShrink: 0 }} />
      {etatLabels[etat] || etat}
    </Box>
  );
}
