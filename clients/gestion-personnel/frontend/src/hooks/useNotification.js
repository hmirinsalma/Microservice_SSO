import { useState, useCallback } from 'react';

export function useNotification() {
  const [notification, setNotification] = useState({ open: false, message: '', severity: 'success' });

  const showSuccess = useCallback((message) =>
    setNotification({ open: true, message, severity: 'success' }), []);

  const showError = useCallback((message) =>
    setNotification({ open: true, message, severity: 'error' }), []);

  const showWarning = useCallback((message) =>
    setNotification({ open: true, message, severity: 'warning' }), []);

  const handleClose = useCallback(() =>
    setNotification(prev => ({ ...prev, open: false })), []);

  return { notification, showSuccess, showError, showWarning, handleClose };
}
