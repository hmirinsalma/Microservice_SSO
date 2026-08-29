import { createTheme, alpha } from '@mui/material/styles'

const theme = createTheme({
  palette: {
    mode: 'light',
    primary:   { main: '#1e3a5f', light: '#2d5282', dark: '#152c47', contrastText: '#fff' },
    secondary: { main: '#0ea5e9', light: '#38bdf8', dark: '#0284c7', contrastText: '#fff' },
    success:   { main: '#16a34a', light: '#22c55e', dark: '#15803d' },
    warning:   { main: '#d97706', light: '#f59e0b', dark: '#b45309' },
    error:     { main: '#dc2626', light: '#ef4444', dark: '#b91c1c' },
    info:      { main: '#0ea5e9', light: '#38bdf8', dark: '#0284c7' },
    background:{ default: '#f1f5f9', paper: '#ffffff' },
    text:      { primary: '#0f172a', secondary: '#64748b', disabled: '#94a3b8' },
    divider:   '#e2e8f0',
    grey: {
      50:  '#f8fafc', 100: '#f1f5f9', 200: '#e2e8f0',
      300: '#cbd5e1', 400: '#94a3b8', 500: '#64748b',
      600: '#475569', 700: '#334155', 800: '#1e293b', 900: '#0f172a',
    },
  },
  typography: {
    fontFamily: '"Inter","Segoe UI","Roboto","Helvetica Neue",Arial,sans-serif',
    h4: { fontWeight: 700, letterSpacing: '-0.02em' },
    h5: { fontWeight: 700, letterSpacing: '-0.01em' },
    h6: { fontWeight: 600 },
    subtitle1: { fontWeight: 600, lineHeight: 1.4 },
    subtitle2: { fontWeight: 600, fontSize: '0.8rem' },
    body1:     { fontSize: '0.9rem', lineHeight: 1.6 },
    body2:     { fontSize: '0.8rem', lineHeight: 1.5 },
    caption:   { fontSize: '0.72rem', letterSpacing: '0.02em' },
    overline:  { fontSize: '0.65rem', fontWeight: 600, letterSpacing: '0.1em' },
  },
  shape: { borderRadius: 8 },
  shadows: [
    'none',
    '0 1px 3px rgba(0,0,0,0.06),0 1px 2px rgba(0,0,0,0.04)',
    '0 4px 6px -1px rgba(0,0,0,0.07),0 2px 4px -2px rgba(0,0,0,0.05)',
    '0 10px 15px -3px rgba(0,0,0,0.07),0 4px 6px -4px rgba(0,0,0,0.05)',
    '0 20px 25px -5px rgba(0,0,0,0.08),0 8px 10px -6px rgba(0,0,0,0.04)',
    ...Array(20).fill('none'),
  ],
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: { backgroundColor: '#f1f5f9', scrollbarWidth: 'thin',
          '&::-webkit-scrollbar': { width: 6 },
          '&::-webkit-scrollbar-track': { background: '#f1f5f9' },
          '&::-webkit-scrollbar-thumb': { background: '#cbd5e1', borderRadius: 3 },
        }
      }
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: { textTransform: 'none', fontWeight: 600, borderRadius: 6, fontSize: '0.82rem', letterSpacing: '0.01em' },
        sizeSmall: { fontSize: '0.75rem', padding: '4px 10px' },
        contained: { boxShadow: '0 1px 3px rgba(0,0,0,0.12)' },
      }
    },
    MuiCard: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: { border: '1px solid #e2e8f0', borderRadius: 10,
          boxShadow: '0 1px 3px rgba(0,0,0,0.04)', transition: 'box-shadow .2s',
          '&:hover': { boxShadow: '0 4px 12px rgba(0,0,0,0.08)' } }
      }
    },
    MuiCardContent: {
      styleOverrides: { root: { padding: '20px', '&:last-child': { paddingBottom: '20px' } } }
    },
    MuiChip: {
      styleOverrides: {
        root: { fontWeight: 600, fontSize: '0.72rem', height: 22, borderRadius: 4,
          letterSpacing: '0.02em' },
        sizeSmall: { height: 20 }
      }
    },
    MuiTableCell: {
      styleOverrides: {
        head: { fontWeight: 600, fontSize: '0.72rem', textTransform: 'uppercase',
          letterSpacing: '0.06em', color: '#64748b', backgroundColor: '#f8fafc',
          borderBottom: '1px solid #e2e8f0', padding: '10px 16px' },
        body: { fontSize: '0.82rem', color: '#1e293b', padding: '10px 16px',
          borderBottom: '1px solid #f1f5f9' }
      }
    },
    MuiTextField: {
      defaultProps: { size: 'small', variant: 'outlined' },
      styleOverrides: {
        root: { '& .MuiOutlinedInput-root': { borderRadius: 6, fontSize: '0.85rem',
          backgroundColor: '#fff',
          '& fieldset': { borderColor: '#e2e8f0' },
          '&:hover fieldset': { borderColor: '#94a3b8' },
          '&.Mui-focused fieldset': { borderColor: '#1e3a5f', borderWidth: 1.5 } } }
      }
    },
    MuiSelect: {
      styleOverrides: {
        root: { borderRadius: 6, fontSize: '0.85rem' }
      }
    },
    MuiMenuItem: {
      styleOverrides: {
        root: { fontSize: '0.82rem', borderRadius: 4, margin: '1px 4px', padding: '6px 10px',
          '&.Mui-selected': { backgroundColor: alpha('#1e3a5f', 0.08) },
          '&:hover': { backgroundColor: '#f1f5f9' } }
      }
    },
    MuiListItemButton: {
      styleOverrides: {
        root: { borderRadius: 6, margin: '1px 0', transition: 'all .15s',
          '&.Mui-selected': { backgroundColor: alpha('#0ea5e9', 0.12),
            '&:hover': { backgroundColor: alpha('#0ea5e9', 0.16) } },
          '&:hover': { backgroundColor: alpha('#0ea5e9', 0.06) } }
      }
    },
    MuiTooltip: {
      styleOverrides: {
        tooltip: { fontSize: '0.72rem', backgroundColor: '#1e293b', borderRadius: 4 }
      }
    },
    MuiDivider: {
      styleOverrides: { root: { borderColor: '#e2e8f0' } }
    },
    MuiPaper: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: { backgroundImage: 'none', border: '1px solid #e2e8f0' }
      }
    },
    MuiDataGrid: {
      styleOverrides: {
        root: { border: 'none', fontSize: '0.82rem',
          '& .MuiDataGrid-columnHeaders': { backgroundColor: '#f8fafc', borderBottom: '1px solid #e2e8f0' },
          '& .MuiDataGrid-cell': { borderBottom: '1px solid #f1f5f9' },
          '& .MuiDataGrid-row:hover': { backgroundColor: '#f8fafc' },
          '& .MuiDataGrid-footerContainer': { borderTop: '1px solid #e2e8f0', backgroundColor: '#f8fafc' },
        }
      }
    },
    MuiLinearProgress: {
      styleOverrides: {
        root: { borderRadius: 4, height: 5, backgroundColor: '#e2e8f0' },
        bar:  { borderRadius: 4 }
      }
    },
    MuiAppBar: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: { borderBottom: '1px solid #e2e8f0' }
      }
    },
    MuiBadge: {
      styleOverrides: {
        badge: { fontSize: '0.65rem', height: 16, minWidth: 16, padding: '0 4px' }
      }
    },
    MuiSkeleton: {
      styleOverrides: { root: { borderRadius: 6, backgroundColor: '#e2e8f0' } }
    }
  }
})

export default theme
