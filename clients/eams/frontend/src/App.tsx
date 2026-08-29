import React from 'react';
import { ThemeProvider, CssBaseline } from '@mui/material';
import { oneeTheme } from './theme/oneeTheme';
import { AuthProvider } from './contexts/AuthContext';
import { SnackbarProvider } from './contexts/SnackbarContext';
import AppRouter from './router/AppRouter';

export default function App() {
  return (
    <ThemeProvider theme={oneeTheme}>
      <CssBaseline />
      <AuthProvider>
        <SnackbarProvider>
          <AppRouter />
        </SnackbarProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}
