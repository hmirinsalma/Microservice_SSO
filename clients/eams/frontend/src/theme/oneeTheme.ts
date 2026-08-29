import { createTheme, alpha } from '@mui/material/styles';

export const oneeTheme = createTheme({
  palette: {
    mode: 'light',
    primary:   { main: '#0066CC', light: '#3385D6', dark: '#004999', contrastText: '#fff' },
    secondary: { main: '#00A86B', light: '#33B989', dark: '#007A4D', contrastText: '#fff' },
    error:     { main: '#E53E3E' },
    warning:   { main: '#ED8936' },
    success:   { main: '#48BB78' },
    info:      { main: '#4299E1' },
    background: { default: '#F0F4F8', paper: '#FFFFFF' },
    text: { primary: '#1A202C', secondary: '#718096' },
    divider: '#E2E8F0',
  },
  typography: {
    fontFamily: '"Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
    h1: { fontWeight: 800, letterSpacing: '-0.02em' },
    h2: { fontWeight: 700, letterSpacing: '-0.01em' },
    h3: { fontWeight: 700, letterSpacing: '-0.01em' },
    h4: { fontWeight: 700, letterSpacing: '-0.01em' },
    h5: { fontWeight: 600 },
    h6: { fontWeight: 600 },
    subtitle1: { fontWeight: 500 },
    body2: { lineHeight: 1.6 },
    button: { fontWeight: 600, textTransform: 'none', letterSpacing: '0' },
  },
  shape: { borderRadius: 12 },
  shadows: [
    'none',
    '0 1px 3px rgba(0,0,0,0.06), 0 1px 2px rgba(0,0,0,0.04)',
    '0 2px 6px rgba(0,0,0,0.06)',
    '0 4px 12px rgba(0,0,0,0.08)',
    '0 6px 16px rgba(0,0,0,0.08)',
    '0 8px 24px rgba(0,0,0,0.10)',
    '0 10px 30px rgba(0,0,0,0.10)',
    '0 12px 36px rgba(0,0,0,0.12)',
    '0 14px 40px rgba(0,0,0,0.12)',
    '0 16px 48px rgba(0,0,0,0.14)',
    '0 18px 52px rgba(0,0,0,0.14)',
    '0 20px 56px rgba(0,0,0,0.16)',
    '0 22px 60px rgba(0,0,0,0.16)',
    '0 24px 64px rgba(0,0,0,0.18)',
    '0 26px 68px rgba(0,0,0,0.18)',
    '0 28px 72px rgba(0,0,0,0.20)',
    '0 30px 76px rgba(0,0,0,0.20)',
    '0 32px 80px rgba(0,0,0,0.22)',
    '0 34px 84px rgba(0,0,0,0.22)',
    '0 36px 88px rgba(0,0,0,0.24)',
    '0 38px 92px rgba(0,0,0,0.24)',
    '0 40px 96px rgba(0,0,0,0.26)',
    '0 42px 100px rgba(0,0,0,0.26)',
    '0 44px 104px rgba(0,0,0,0.28)',
    '0 46px 108px rgba(0,0,0,0.28)',
  ],
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 10, fontWeight: 600, fontSize: '0.875rem',
          padding: '8px 20px', boxShadow: 'none',
          '&:hover': { boxShadow: '0 4px 12px rgba(0,102,204,0.25)' },
        },
        contained: {
          '&:hover': { transform: 'translateY(-1px)' },
          transition: 'all 0.2s ease',
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 16, boxShadow: '0 1px 3px rgba(0,0,0,0.06), 0 1px 2px rgba(0,0,0,0.04)',
          border: '1px solid #E2E8F0',
          '&:hover': { boxShadow: '0 4px 12px rgba(0,0,0,0.10)' },
          transition: 'box-shadow 0.2s ease',
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            borderRadius: 10,
            '& fieldset': { borderColor: '#E2E8F0' },
            '&:hover fieldset': { borderColor: '#CBD5E0' },
            '&.Mui-focused fieldset': { borderColor: '#0066CC', borderWidth: '1.5px' },
          },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { borderRadius: 8, fontWeight: 600, fontSize: '0.75rem' },
      },
    },
    MuiTableHead: {
      styleOverrides: {
        root: { '& .MuiTableCell-head': { fontWeight: 600, fontSize: '0.8rem', color: '#718096', letterSpacing: '0.05em', textTransform: 'uppercase', background: '#F7FAFC', borderBottom: '2px solid #E2E8F0' } },
      },
    },
    MuiTableRow: {
      styleOverrides: {
        root: { '&:hover': { background: '#F7FAFC' }, '&:last-child td': { border: 0 }, transition: 'background 0.15s' },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: { padding: '14px 16px', borderColor: '#F0F4F8', fontSize: '0.875rem' },
      },
    },
    MuiSelect: {
      styleOverrides: {
        outlined: { borderRadius: 10 },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: { backgroundImage: 'none' },
      },
    },
    MuiLinearProgress: {
      styleOverrides: {
        root: { borderRadius: 6, height: 6 },
      },
    },
    MuiTooltip: {
      styleOverrides: {
        tooltip: { borderRadius: 8, fontSize: '0.78rem', background: '#1A202C' },
      },
    },
  },
});

export const etatColors: Record<string, { bg: string; color: string; border: string }> = {
  Disponible:     { bg: '#F0FFF4', color: '#276749', border: '#9AE6B4' },
  En_maintenance: { bg: '#FFFAF0', color: '#9C4221', border: '#FBD38D' },
  En_panne:       { bg: '#FFF5F5', color: '#9B2C2C', border: '#FEB2B2' },
  Hors_service:   { bg: '#F7FAFC', color: '#4A5568', border: '#CBD5E0' },
  Reserve:        { bg: '#EBF8FF', color: '#2C5282', border: '#90CDF4' },
};
