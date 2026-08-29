import React, { createContext, useContext, useState, ReactNode, useCallback } from 'react';
import { Snackbar, Alert, AlertColor } from '@mui/material';

interface SnackMsg { message: string; severity: AlertColor }

interface SnackbarContextType {
  showSuccess: (msg: string) => void;
  showError: (msg: string) => void;
  showInfo: (msg: string) => void;
}

const SnackbarContext = createContext<SnackbarContextType | undefined>(undefined);

export const SnackbarProvider = ({ children }: { children: ReactNode }) => {
  const [snack, setSnack] = useState<SnackMsg | null>(null);
  const [open, setOpen] = useState(false);

  const show = useCallback((message: string, severity: AlertColor) => {
    setSnack({ message, severity });
    setOpen(true);
  }, []);

  const showSuccess = useCallback((msg: string) => show(msg, 'success'), [show]);
  const showError   = useCallback((msg: string) => show(msg, 'error'),   [show]);
  const showInfo    = useCallback((msg: string) => show(msg, 'info'),    [show]);

  return (
    <SnackbarContext.Provider value={{ showSuccess, showError, showInfo }}>
      {children}
      <Snackbar open={open} autoHideDuration={4000} onClose={() => setOpen(false)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert onClose={() => setOpen(false)} severity={snack?.severity || 'info'} sx={{ width: '100%' }}>
          {snack?.message}
        </Alert>
      </Snackbar>
    </SnackbarContext.Provider>
  );
};

export const useSnackbar = () => {
  const ctx = useContext(SnackbarContext);
  if (!ctx) throw new Error('useSnackbar must be used within SnackbarProvider');
  return ctx;
};
