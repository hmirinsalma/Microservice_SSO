import React from 'react';
import { Box, Skeleton } from '@mui/material';

export default function SkeletonTable({ rows = 5 }: { rows?: number }) {
  return (
    <Box sx={{ p: 2 }}>
      <Skeleton variant="rectangular" height={44} sx={{ borderRadius: 2, mb: 2 }} />
      {Array.from({ length: rows }).map((_, i) => (
        <Skeleton key={i} variant="rectangular" height={56} sx={{ borderRadius: 1, mb: 1, opacity: 1 - i * 0.12 }} />
      ))}
    </Box>
  );
}
