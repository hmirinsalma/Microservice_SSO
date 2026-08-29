import React from 'react';
import { Box, Typography, Breadcrumbs, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { IconChevronRight, IconHome } from '@tabler/icons-react';

interface Crumb { label: string; to?: string }
interface Props { title: string; subtitle?: string; crumbs?: Crumb[]; action?: React.ReactNode }

export default function PageHeader({ title, subtitle, crumbs, action }: Props) {
  return (
    <Box sx={{ mb: 3 }}>
      {crumbs && (
        <Breadcrumbs separator={<IconChevronRight size={14} />} sx={{ mb: 1 }}>
          {crumbs.map((c, i) =>
            c.to
              ? <Link key={i} component={RouterLink} to={c.to} underline="hover"
                  sx={{ display: 'flex', alignItems: 'center', gap: 0.5, color: 'text.secondary', fontSize: '0.8rem', fontWeight: 500, '&:hover': { color: 'primary.main' } }}>
                  {i === 0 && <IconHome size={14} />}{c.label}
                </Link>
              : <Typography key={i} sx={{ fontSize: '0.8rem', fontWeight: 600, color: 'text.primary' }}>{c.label}</Typography>
          )}
        </Breadcrumbs>
      )}
      <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Typography variant="h5" sx={{ fontWeight: 700, color: 'text.primary', lineHeight: 1.2 }}>{title}</Typography>
          {subtitle && <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>{subtitle}</Typography>}
        </Box>
        {action}
      </Box>
    </Box>
  );
}
