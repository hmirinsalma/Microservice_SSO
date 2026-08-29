import React, { createContext, useContext, useState, useCallback } from 'react'
import { Snackbar, Alert } from '@mui/material'

const SnackbarContext = createContext(null)

export function SnackbarProvider({ children }) {
  const [snack, setSnack] = useState({ open: false, message: '', severity: 'success' })

  const notify = useCallback((message, severity = 'success') => {
    setSnack({ open: true, message, severity })
  }, [])

  const close = () => setSnack(s => ({ ...s, open: false }))

  return (
    <SnackbarContext.Provider value={{ notify }}>
      {children}
      <Snackbar open={snack.open} autoHideDuration={4000} onClose={close}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert onClose={close} severity={snack.severity} variant="filled" sx={{ width: '100%' }}>
          {snack.message}
        </Alert>
      </Snackbar>
    </SnackbarContext.Provider>
  )
}

export const useSnackbar = () => useContext(SnackbarContext)
