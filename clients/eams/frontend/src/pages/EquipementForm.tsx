import React, { useEffect, useState } from 'react';
import {
  Box, Card, CardContent, Grid, TextField, Button, MenuItem,
  Select, FormControl, InputLabel, CircularProgress
} from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { getEquipement, createEquipement, updateEquipement } from '../api/equipementsApi';
import { getCategories } from '../api/categoriesApi';
import { getUsers } from '../api/usersApi';
import { CategorieDto, UserDto, EquipementEtat } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';

const etats: EquipementEtat[] = ['Disponible', 'En_maintenance', 'En_panne', 'Hors_service', 'Reserve'];
const etatLabels: Record<string, string> = { Disponible: 'Disponible', En_maintenance: 'En maintenance', En_panne: 'En panne', Hors_service: 'Hors service', Reserve: 'Réservé' };

export default function EquipementForm() {
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const { showSuccess, showError } = useSnackbar();
  const [categories, setCategories] = useState<CategorieDto[]>([]);
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const { register, handleSubmit, control, reset } = useForm<Record<string, unknown>>();

  useEffect(() => {
    Promise.all([getCategories(), getUsers()]).then(([cRes, uRes]) => {
      setCategories(cRes.data.data);
      setUsers(uRes.data.data);
    });
    if (isEdit && id) {
      getEquipement(id).then(r => {
        const e = r.data.data;
        reset({
          nom: e.nom, categorieId: e.categorieId, type: e.type, marque: e.marque, modele: e.modele,
          numeroSerie: e.numeroSerie, localisation: e.localisation, serviceId: e.serviceId,
          responsableId: e.responsableId, dateInstallation: e.dateInstallation?.substring(0, 10),
          etat: e.etat, dateFinGarantie: e.dateFinGarantie?.substring(0, 10) || '',
          valeurAcquisition: e.valeurAcquisition || '', fournisseur: e.fournisseur || '', description: e.description || ''
        });
      }).finally(() => setLoading(false));
    }
  }, [id]);

  const onSubmit = async (d: Record<string, unknown>) => {
    setSaving(true);
    try {
      if (isEdit && id) { await updateEquipement(id, d); showSuccess('Équipement mis à jour.'); }
      else { await createEquipement(d); showSuccess('Équipement créé.'); }
      navigate('/equipements');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      showError(msg || 'Erreur.');
    } finally { setSaving(false); }
  };

  if (loading) return <Box sx={{ p: 3 }}><SkeletonTable rows={6} /></Box>;

  return (
    <Box>
      <PageHeader
        title={isEdit ? 'Modifier l\'équipement' : 'Nouvel équipement'}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Équipements', to: '/equipements' }, { label: isEdit ? 'Modifier' : 'Nouveau' }]}
      />
      <Card>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}><TextField {...register('nom')} label="Nom *" fullWidth required /></Grid>
              <Grid item xs={12} md={6}>
                <Controller name="categorieId" control={control} defaultValue="" render={({ field }) => (
                  <FormControl fullWidth required>
                    <InputLabel>Catégorie *</InputLabel>
                    <Select {...field} label="Catégorie *">
                      {categories.map(c => <MenuItem key={c.id} value={c.id}>{c.nom}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6} md={4}><TextField {...register('type')} label="Type *" fullWidth required /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('marque')} label="Marque *" fullWidth required /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('modele')} label="Modèle *" fullWidth required /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('numeroSerie')} label="Numéro de série *" fullWidth required /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('localisation')} label="Localisation *" fullWidth required /></Grid>
              <Grid item xs={6} md={4}>
                <Controller name="etat" control={control} defaultValue="Disponible" render={({ field }) => (
                  <FormControl fullWidth>
                    <InputLabel>État</InputLabel>
                    <Select {...field} label="État">
                      {etats.map(e => <MenuItem key={e} value={e}>{etatLabels[e]}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6} md={4}>
                <Controller name="responsableId" control={control} defaultValue="" render={({ field }) => (
                  <FormControl fullWidth required>
                    <InputLabel>Responsable *</InputLabel>
                    <Select {...field} label="Responsable *">
                      {users.map(u => <MenuItem key={u.id} value={u.id}>{u.prenom} {u.nom}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6} md={4}><TextField {...register('dateInstallation')} label="Date d'installation *" type="date" fullWidth required InputLabelProps={{ shrink: true }} /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('dateFinGarantie')} label="Fin de garantie" type="date" fullWidth InputLabelProps={{ shrink: true }} /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('valeurAcquisition')} label="Valeur d'acquisition (MAD)" type="number" fullWidth /></Grid>
              <Grid item xs={6} md={4}><TextField {...register('fournisseur')} label="Fournisseur" fullWidth /></Grid>
              <Grid item xs={12}><TextField {...register('description')} label="Description" fullWidth multiline rows={3} /></Grid>
              <Grid item xs={12} sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                <Button onClick={() => navigate('/equipements')}>Annuler</Button>
                <Button type="submit" variant="contained" disabled={saving}>
                  {saving ? <CircularProgress size={22} /> : (isEdit ? 'Enregistrer' : 'Créer')}
                </Button>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
}
