import { useEffect, useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, Button, CircularProgress, Box, MenuItem,
  Grid, Alert, Divider, Typography, IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import employesApi from '../../api/employesApi';
import directionsApi from '../../api/directionsApi';
import servicesApi from '../../api/servicesApi';
import { extractErrorMessage } from '../../api/apiHelpers';

const STATUTS = ['Actif', 'Inactif', 'Suspendu'];

// Postes par direction (nom exact)
const POSTES_PAR_DIRECTION = {
  'Direction RH': [
    'Responsable RH', 'Chargé de Recrutement', 'Chargé de Formation',
    'Gestionnaire de Paie', 'Responsable Paie', 'Assistant RH',
  ],
  'Direction Technique': [
    'Ingénieur Technique', 'Technicien Maintenance', 'Chef de Chantier',
    'Responsable Exploitation', 'Ingénieur Exploitation', 'Agent Technique',
  ],
  'Direction Informatique': [
    'Développeur Full Stack', 'Développeur Frontend', 'Développeur Backend',
    'Ingénieur DevOps', 'Architecte Logiciel', 'Chef de Projet IT',
    'Analyste Systèmes', 'Administrateur Réseau', 'Administrateur Base de Données',
    'Responsable Sécurité IT',
  ],
  'Direction Patrimoine': [
    'Gestionnaire Immobilier', 'Responsable Logistique', 'Chargé des Achats',
    'Agent de Patrimoine', 'Technicien Immobilier',
  ],
};

// Postes communs toujours disponibles
const POSTES_COMMUNS = [
  'Directeur', 'Directeur Adjoint', 'Chef de Service',
  'Responsable de Département', 'Manager',
  'Assistant Administratif', 'Secrétaire de Direction',
  'Comptable', 'Juriste', 'Auditeur Interne',
];

const INIT = {
  matricule: '', nom: '', prenom: '', email: '', telephone: '',
  dateEmbauche: '', poste: '', statut: 'Actif', directionId: '', serviceId: '',
};

export default function EmployeFormDialog({ open, employe, onClose, onSuccess }) {
  const [form, setForm]             = useState(INIT);
  const [directions, setDirections] = useState([]);
  const [services, setServices]     = useState([]);
  const [errors, setErrors]         = useState({});
  const [submitError, setSubmitError] = useState('');
  const [loading, setLoading]       = useState(false);
  const isEdit = !!employe;

  useEffect(() => {
    directionsApi.getAll().then(({ data }) => setDirections(data)).catch(() => {});
  }, []);

  useEffect(() => {
    if (form.directionId) {
      servicesApi.getByDirection(form.directionId)
        .then(({ data }) => setServices(data)).catch(() => setServices([]));
    } else { setServices([]); }
  }, [form.directionId]);

  useEffect(() => {
    if (open) {
      setForm(employe ? {
        matricule: employe.matricule, nom: employe.nom, prenom: employe.prenom,
        email: employe.email, telephone: employe.telephone || '',
        dateEmbauche: employe.dateEmbauche ? employe.dateEmbauche.split('T')[0] : '',
        poste: employe.poste, statut: employe.statut,
        directionId: employe.directionId, serviceId: employe.serviceId,
      } : INIT);
      setErrors({}); setSubmitError('');
    }
  }, [employe, open]);

  // Calcule la liste de postes selon la direction choisie
  const getPostes = () => {
    const dir = directions.find(d => d.id === Number(form.directionId));
    const specific = dir ? (POSTES_PAR_DIRECTION[dir.nom] || []) : [];
    return [...specific, ...POSTES_COMMUNS.filter(p => !specific.includes(p))];
  };

  const validate = () => {
    const e = {};
    if (!isEdit && !form.matricule.trim()) e.matricule = 'Requis';
    if (!form.nom.trim())    e.nom    = 'Requis';
    if (!form.prenom.trim()) e.prenom = 'Requis';
    if (!form.email.trim())  e.email  = 'Requis';
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = 'Email invalide';
    if (!form.dateEmbauche)  e.dateEmbauche = 'Requise';
    if (!form.poste)         e.poste  = 'Requis';
    if (!form.directionId)   e.directionId = 'Requise';
    if (!form.serviceId)     e.serviceId   = 'Requis';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setLoading(true); setSubmitError('');
    try {
      if (isEdit) {
        await employesApi.update(employe.id, {
          nom: form.nom, prenom: form.prenom, email: form.email,
          telephone: form.telephone || null,
          dateEmbauche: new Date(form.dateEmbauche).toISOString(),
          poste: form.poste, statut: form.statut,
          directionId: Number(form.directionId), serviceId: Number(form.serviceId),
        });
      } else {
        await employesApi.create({
          ...form, telephone: form.telephone || null,
          dateEmbauche: new Date(form.dateEmbauche).toISOString(),
          directionId: Number(form.directionId), serviceId: Number(form.serviceId),
        });
      }
      onSuccess(isEdit ? 'Employé modifié avec succès.' : 'Employé créé avec succès.');
      onClose();
    } catch (err) { setSubmitError(extractErrorMessage(err)); }
    finally { setLoading(false); }
  };

  const set = (field) => (e) => setForm(prev => {
    const next = { ...prev, [field]: e.target.value };
    // Reset cascadé
    if (field === 'directionId') { next.serviceId = ''; next.poste = ''; }
    return next;
  });

  const postes = getPostes();

  return (
    <Dialog open={open} onClose={loading ? undefined : onClose} maxWidth="md" fullWidth
      PaperProps={{ sx: { borderRadius: 3, m: 2 } }}>
      <DialogTitle sx={{ px: 3, py: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h6" fontWeight={700}>
          {isEdit ? `Modifier — ${employe?.nom} ${employe?.prenom}` : 'Nouvel employé'}
        </Typography>
        <IconButton size="small" onClick={onClose} disabled={loading}><CloseIcon fontSize="small" /></IconButton>
      </DialogTitle>
      <Divider />

      <DialogContent sx={{ px: 3, py: 3 }}>
        {submitError && <Alert severity="error" sx={{ mb: 3, borderRadius: 2 }}>{submitError}</Alert>}

        {/* IDENTITÉ */}
        <Typography variant="caption" fontWeight={700} color="text.secondary"
          sx={{ letterSpacing: 0.8, mb: 1.5, display: 'block' }}>
          IDENTITÉ
        </Typography>
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={12} sm={4}>
            <TextField fullWidth label="Matricule" required size="small"
              value={form.matricule} onChange={set('matricule')}
              error={!!errors.matricule} helperText={errors.matricule || (isEdit ? 'Non modifiable' : '')}
              disabled={loading || isEdit} />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField fullWidth label="Nom" required size="small"
              value={form.nom} onChange={set('nom')}
              error={!!errors.nom} helperText={errors.nom} disabled={loading} />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField fullWidth label="Prénom" required size="small"
              value={form.prenom} onChange={set('prenom')}
              error={!!errors.prenom} helperText={errors.prenom} disabled={loading} />
          </Grid>
        </Grid>

        {/* CONTACT */}
        <Typography variant="caption" fontWeight={700} color="text.secondary"
          sx={{ letterSpacing: 0.8, mb: 1.5, display: 'block' }}>
          CONTACT
        </Typography>
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={12} sm={6}>
            <TextField fullWidth label="Email professionnel" required size="small" type="email"
              value={form.email} onChange={set('email')}
              error={!!errors.email} helperText={errors.email} disabled={loading} />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField fullWidth label="Téléphone (optionnel)" size="small"
              value={form.telephone} onChange={set('telephone')} disabled={loading} />
          </Grid>
        </Grid>

        {/* ORGANISATION — doit venir AVANT poste pour pouvoir filtrer */}
        <Typography variant="caption" fontWeight={700} color="text.secondary"
          sx={{ letterSpacing: 0.8, mb: 1.5, display: 'block' }}>
          ORGANISATION
        </Typography>
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={12} sm={6}>
            <TextField select fullWidth label="Direction" required size="small"
              value={form.directionId} onChange={set('directionId')}
              error={!!errors.directionId} helperText={errors.directionId} disabled={loading}>
              <MenuItem value=""><em>— Sélectionner une direction —</em></MenuItem>
              {directions.map(d => <MenuItem key={d.id} value={d.id}>{d.nom}</MenuItem>)}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField select fullWidth label="Service" required size="small"
              value={form.serviceId} onChange={set('serviceId')}
              error={!!errors.serviceId}
              helperText={errors.serviceId || (!form.directionId ? '← Sélectionner d\'abord une direction' : '')}
              disabled={loading || !form.directionId}>
              <MenuItem value=""><em>— Sélectionner un service —</em></MenuItem>
              {services.map(s => <MenuItem key={s.id} value={s.id}>{s.nom}</MenuItem>)}
            </TextField>
          </Grid>
        </Grid>

        {/* POSTE & STATUT */}
        <Typography variant="caption" fontWeight={700} color="text.secondary"
          sx={{ letterSpacing: 0.8, mb: 1.5, display: 'block' }}>
          POSTE & STATUT
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={4}>
            {/* Date — utilise un input natif pour éviter le chevauchement label/placeholder */}
            <Box>
              <Typography variant="caption" color="text.secondary" fontWeight={500} display="block" mb={0.5}>
                Date d'embauche <Box component="span" sx={{ color: 'error.main' }}>*</Box>
              </Typography>
              <Box
                component="input"
                type="date"
                value={form.dateEmbauche}
                onChange={(e) => setForm(prev => ({ ...prev, dateEmbauche: e.target.value }))}
                disabled={loading}
                style={{
                  width: '100%',
                  height: 40,
                  padding: '0 12px',
                  border: errors.dateEmbauche ? '1px solid #EF4444' : '1px solid #E2E8F0',
                  borderRadius: 8,
                  fontSize: 14,
                  fontFamily: 'inherit',
                  color: '#0F172A',
                  backgroundColor: '#fff',
                  outline: 'none',
                  cursor: 'pointer',
                  boxSizing: 'border-box',
                }}
                onFocus={(e) => e.target.style.borderColor = '#1565C0'}
                onBlur={(e) => e.target.style.borderColor = errors.dateEmbauche ? '#EF4444' : '#E2E8F0'}
              />
              {errors.dateEmbauche && (
                <Typography variant="caption" color="error.main" display="block" mt={0.5}>
                  {errors.dateEmbauche}
                </Typography>
              )}
            </Box>
          </Grid>
          <Grid item xs={12} sm={5}>
            <TextField select fullWidth label="Poste / Fonction" required size="small"
              value={form.poste} onChange={set('poste')}
              error={!!errors.poste}
              helperText={errors.poste || (!form.directionId ? '← Sélectionner d\'abord une direction' : '')}
              disabled={loading || !form.directionId}>
              <MenuItem value=""><em>— Sélectionner un poste —</em></MenuItem>
              {postes.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField select fullWidth label="Statut" required size="small"
              value={form.statut} onChange={set('statut')} disabled={loading}>
              {STATUTS.map(s => <MenuItem key={s} value={s}>{s}</MenuItem>)}
            </TextField>
          </Grid>
        </Grid>
      </DialogContent>

      <Divider />
      <DialogActions sx={{ px: 3, py: 2, gap: 1 }}>
        <Button onClick={onClose} disabled={loading} variant="outlined" sx={{ borderRadius: 2 }}>
          Annuler
        </Button>
        <Button variant="contained" onClick={handleSubmit} disabled={loading}
          sx={{ borderRadius: 2, minWidth: 150 }}
          startIcon={loading ? <CircularProgress size={16} color="inherit" /> : null}>
          {loading ? 'Enregistrement...' : isEdit ? 'Enregistrer' : 'Créer l\'employé'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
