import { useEffect, useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, Button, CircularProgress, Box, MenuItem,
  Alert, Divider, Typography, IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import servicesApi from '../../api/servicesApi';
import directionsApi from '../../api/directionsApi';
import { extractErrorMessage } from '../../api/apiHelpers';

const INIT = { nom: '', description: '', directionId: '' };

export default function ServiceFormDialog({ open, service, onClose, onSuccess }) {
  const [form, setForm]           = useState(INIT);
  const [directions, setDirections] = useState([]);
  const [errors, setErrors]       = useState({});
  const [submitError, setSubmitError] = useState('');
  const [loading, setLoading]     = useState(false);
  const isEdit = !!service;

  useEffect(() => {
    directionsApi.getAll().then(({ data }) => setDirections(data)).catch(() => {});
  }, []);

  useEffect(() => {
    if (open) {
      setForm(service ? { nom: service.nom, description: service.description || '', directionId: service.directionId } : INIT);
      setErrors({}); setSubmitError('');
    }
  }, [service, open]);

  const validate = () => {
    const e = {};
    if (!form.nom.trim())    e.nom = 'Le nom est requis.';
    if (!form.directionId)   e.directionId = 'La direction est requise.';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setLoading(true); setSubmitError('');
    try {
      const payload = { ...form, directionId: Number(form.directionId) };
      if (isEdit) await servicesApi.update(service.id, payload);
      else await servicesApi.create(payload);
      onSuccess(isEdit ? 'Service modifié.' : 'Service créé.');
      onClose();
    } catch (err) { setSubmitError(extractErrorMessage(err)); }
    finally { setLoading(false); }
  };

  return (
    <Dialog open={open} onClose={loading ? undefined : onClose} maxWidth="sm" fullWidth
      PaperProps={{ sx: { borderRadius: 3, m: 2 } }}>
      <DialogTitle sx={{ px: 3, py: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h6" fontWeight={700}>
          {isEdit ? 'Modifier le service' : 'Nouveau service'}
        </Typography>
        <IconButton size="small" onClick={onClose} disabled={loading}><CloseIcon fontSize="small" /></IconButton>
      </DialogTitle>
      <Divider />
      <DialogContent sx={{ px: 3, py: 3 }}>
        {submitError && <Alert severity="error" sx={{ mb: 2.5, borderRadius: 2 }}>{submitError}</Alert>}
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
          <TextField select fullWidth label="Direction" required size="small"
            value={form.directionId} onChange={(e) => setForm({ ...form, directionId: e.target.value })}
            error={!!errors.directionId} helperText={errors.directionId} disabled={loading}>
            <MenuItem value=""><em>— Sélectionner une direction —</em></MenuItem>
            {directions.map(d => <MenuItem key={d.id} value={d.id}>{d.nom}</MenuItem>)}
          </TextField>
          <TextField fullWidth label="Nom du service" required size="small"
            value={form.nom} onChange={(e) => setForm({ ...form, nom: e.target.value })}
            error={!!errors.nom} helperText={errors.nom} disabled={loading} autoFocus />
          <TextField fullWidth label="Description (optionnelle)" size="small" multiline rows={3}
            value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })}
            disabled={loading} helperText={`${form.description.length}/500`} inputProps={{ maxLength: 500 }} />
        </Box>
      </DialogContent>
      <Divider />
      <DialogActions sx={{ px: 3, py: 2, gap: 1 }}>
        <Button onClick={onClose} disabled={loading} variant="outlined" sx={{ borderRadius: 2 }}>Annuler</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={loading} sx={{ borderRadius: 2, minWidth: 120 }}
          startIcon={loading ? <CircularProgress size={16} color="inherit" /> : null}>
          {loading ? 'Enregistrement...' : isEdit ? 'Enregistrer' : 'Créer'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
