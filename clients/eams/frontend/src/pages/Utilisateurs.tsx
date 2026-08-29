import React, { useEffect, useState } from 'react';
import {
  Box, Button, Card, CardContent, Table, TableHead, TableRow, TableCell, TableBody,
  IconButton, Tooltip, Chip, Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, MenuItem, Select, FormControl, InputLabel, Grid, Typography, Avatar, Divider
} from '@mui/material';
import { IconPlus, IconEdit, IconToggleRight, IconToggleLeft } from '@tabler/icons-react';
import { getUsers, createUser, updateUser, toggleActive } from '../api/usersApi';
import { UserDto, UserRole } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import { useForm, Controller } from 'react-hook-form';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';

const roles: UserRole[] = ['Admin_Patrimoine', 'Directeur', 'Chef_de_Service', 'Technicien'];

const roleConfig: Record<string, { label: string; bg: string; color: string }> = {
  Admin_Patrimoine: { label: 'Admin Patrimoine', bg: '#EBF8FF', color: '#2C5282' },
  Directeur:        { label: 'Directeur',         bg: '#FAF5FF', color: '#553C9A' },
  Chef_de_Service:  { label: 'Chef de Service',   bg: '#F0FFF4', color: '#276749' },
  Technicien:       { label: 'Technicien',         bg: '#FFFAF0', color: '#9C4221' },
};

const avatarGrad: Record<string, string> = {
  Admin_Patrimoine: 'linear-gradient(135deg,#0066CC,#004999)',
  Directeur:        'linear-gradient(135deg,#7B2FBE,#5A1F8C)',
  Chef_de_Service:  'linear-gradient(135deg,#00A86B,#007A4D)',
  Technicien:       'linear-gradient(135deg,#ED8936,#C05621)',
};

type FormData = { nom: string; prenom: string; email: string; password: string; telephone: string; poste: string; role: UserRole; serviceId: string; isActive: boolean };

export default function Utilisateurs() {
  const { showSuccess, showError } = useSnackbar();
  const [items, setItems] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<UserDto | null>(null);
  const { register, handleSubmit, reset, control } = useForm<FormData>();

  const load = () => getUsers().then(r => setItems(r.data.data)).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const openCreate = () => {
    setEditing(null);
    reset({ role: 'Technicien', isActive: true });
    setOpen(true);
  };
  const openEdit = (u: UserDto) => {
    setEditing(u);
    reset({ nom: u.nom, prenom: u.prenom, email: u.email, telephone: u.telephone, poste: u.poste, role: u.role, serviceId: u.serviceId || '', isActive: u.isActive });
    setOpen(true);
  };

  const onSubmit = async (d: FormData) => {
    try {
      if (editing) { await updateUser(editing.id, d); showSuccess('Utilisateur mis à jour.'); }
      else { await createUser(d); showSuccess('Utilisateur créé.'); }
      setOpen(false); load();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      showError(msg || 'Erreur.');
    }
  };

  const handleToggle = async (id: string) => {
    try { await toggleActive(id); showSuccess('Statut modifié.'); load(); }
    catch { showError('Erreur.'); }
  };

  return (
    <Box className="fade-in">
      <PageHeader title="Utilisateurs" subtitle={`${items.length} utilisateurs enregistrés`}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Utilisateurs' }]}
        action={
          <Button variant="contained" startIcon={<IconPlus size={16} />} onClick={openCreate}
            sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)', boxShadow: '0 4px 14px rgba(0,102,204,0.3)' }}>
            Nouvel utilisateur
          </Button>
        }
      />

      <Card>
        {loading ? <SkeletonTable /> : (
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Utilisateur</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Rôle</TableCell>
                <TableCell>Service</TableCell>
                <TableCell>Téléphone</TableCell>
                <TableCell>Statut</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {items.map(u => {
                const rc = roleConfig[u.role] || roleConfig.Technicien;
                const ag = avatarGrad[u.role] || avatarGrad.Technicien;
                const initials = `${u.prenom[0]}${u.nom[0]}`.toUpperCase();
                return (
                  <TableRow key={u.id}>
                    <TableCell>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                        <Avatar sx={{ width: 34, height: 34, fontSize: '0.78rem', fontWeight: 800, background: ag }}>
                          {initials}
                        </Avatar>
                        <Box>
                          <Typography sx={{ fontWeight: 600, fontSize: '0.875rem', lineHeight: 1.2 }}>{u.prenom} {u.nom}</Typography>
                          <Typography sx={{ fontSize: '0.75rem', color: '#A0AEC0' }}>{u.poste}</Typography>
                        </Box>
                      </Box>
                    </TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{u.email}</Typography></TableCell>
                    <TableCell>
                      <Box component="span" sx={{ display: 'inline-block', px: 1.2, py: 0.3, borderRadius: '6px', fontSize: '0.72rem', fontWeight: 700, bgcolor: rc.bg, color: rc.color }}>
                        {rc.label}
                      </Box>
                    </TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{u.serviceNom || '—'}</Typography></TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{u.telephone || '—'}</Typography></TableCell>
                    <TableCell>
                      <Box component="span" sx={{ display: 'inline-flex', alignItems: 'center', gap: '5px', px: 1.2, py: 0.4, borderRadius: '20px', fontSize: '0.72rem', fontWeight: 600, bgcolor: u.isActive ? '#F0FFF4' : '#F7FAFC', color: u.isActive ? '#276749' : '#718096', border: `1px solid ${u.isActive ? '#9AE6B4' : '#CBD5E0'}` }}>
                        <Box component="span" sx={{ width: 6, height: 6, borderRadius: '50%', bgcolor: u.isActive ? '#276749' : '#CBD5E0' }} />
                        {u.isActive ? 'Actif' : 'Inactif'}
                      </Box>
                    </TableCell>
                    <TableCell align="right">
                      <Box sx={{ display: 'flex', gap: 0.5, justifyContent: 'flex-end' }}>
                        <Tooltip title="Modifier">
                          <IconButton size="small" onClick={() => openEdit(u)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#EBF8FF', color: '#0066CC' } }}>
                            <IconEdit size={16} />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title={u.isActive ? 'Désactiver' : 'Activer'}>
                          <IconButton size="small" onClick={() => handleToggle(u.id)}
                            sx={{ color: '#4A5568', '&:hover': { bgcolor: u.isActive ? '#FFF5F5' : '#F0FFF4', color: u.isActive ? '#E53E3E' : '#00A86B' } }}>
                            {u.isActive ? <IconToggleLeft size={16} /> : <IconToggleRight size={16} />}
                          </IconButton>
                        </Tooltip>
                      </Box>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        )}
      </Card>

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth
        PaperProps={{ sx: { borderRadius: '16px' } }}>
        <DialogTitle sx={{ pb: 1, fontWeight: 700 }}>
          {editing ? 'Modifier l\'utilisateur' : 'Nouvel utilisateur'}
        </DialogTitle>
        <Divider />
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent sx={{ pt: 2.5 }}>
            <Grid container spacing={2}>
              <Grid item xs={6}><TextField {...register('prenom')} label="Prénom *" fullWidth size="small" required /></Grid>
              <Grid item xs={6}><TextField {...register('nom')} label="Nom *" fullWidth size="small" required /></Grid>
              <Grid item xs={12}><TextField {...register('email')} label="Email *" type="email" fullWidth size="small" required /></Grid>
              {!editing && (
                <Grid item xs={12}>
                  <Alert severity="info" sx={{ borderRadius: '10px', fontSize: '0.82rem' }}>
                    Le compte d'authentification doit être créé séparément via le microservice SSO.
                    Seul le profil métier EAMS est créé ici.
                  </Alert>
                </Grid>
              )}
              <Grid item xs={6}><TextField {...register('telephone')} label="Téléphone" fullWidth size="small" /></Grid>
              <Grid item xs={6}><TextField {...register('poste')} label="Poste" fullWidth size="small" /></Grid>
              <Grid item xs={12}>
                <Controller name="role" control={control} defaultValue="Technicien" render={({ field }) => (
                  <FormControl fullWidth size="small">
                    <InputLabel>Rôle *</InputLabel>
                    <Select {...field} label="Rôle *">
                      {roles.map(r => <MenuItem key={r} value={r}>{roleConfig[r]?.label || r}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions sx={{ px: 3, pb: 2.5, gap: 1 }}>
            <Button onClick={() => setOpen(false)} sx={{ color: '#718096' }}>Annuler</Button>
            <Button type="submit" variant="contained"
              sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)', minWidth: 120 }}>
              {editing ? 'Enregistrer' : 'Créer'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
}
