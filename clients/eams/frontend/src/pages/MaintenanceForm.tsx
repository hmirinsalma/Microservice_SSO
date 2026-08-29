import React, { useEffect, useState } from 'react';
import { Box, Card, CardContent, Grid, TextField, Button, MenuItem, Select, FormControl, InputLabel, CircularProgress } from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { getMaintenance, createMaintenance, updateMaintenance } from '../api/maintenancesApi';
import { getEquipements } from '../api/equipementsApi';
import { getUsers } from '../api/usersApi';
import { EquipementListDto, UserDto } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import PageHeader from '../components/common/PageHeader';

export default function MaintenanceForm() {
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const { showSuccess, showError } = useSnackbar();
  const [equipements, setEquipements] = useState<EquipementListDto[]>([]);
  const [techniciens, setTechniciens] = useState<UserDto[]>([]);
  const [saving, setSaving] = useState(false);
  const { register, handleSubmit, control, reset } = useForm<Record<string, unknown>>();

  useEffect(() => {
    Promise.all([
      getEquipements({ page: 1, pageSize: 300 }),
      getUsers()
    ]).then(([eqRes, uRes]) => {
      setEquipements(eqRes.data.data.items);
      setTechniciens(uRes.data.data.filter((u: UserDto) => u.role === 'Technicien'));
    });
    if (isEdit && id) {
      getMaintenance(id).then(r => {
        const m = r.data.data;
        reset({
          equipementId: m.equipementId, technicienId: m.technicienId,
          type: m.type, statut: m.statut,
          datePlanifiee: m.datePlanifiee?.substring(0, 10),
          dureeMinutes: m.dureeMinutes || '', coutEstime: m.coutEstime || '',
          observations: m.observations || '', piecesRemplacees: m.piecesRemplacees || ''
        });
      });
    }
  }, [id]);

  const onSubmit = async (d: Record<string, unknown>) => {
    setSaving(true);
    try {
      if (isEdit && id) { await updateMaintenance(id, d); showSuccess('Maintenance mise à jour.'); }
      else { await createMaintenance(d); showSuccess('Maintenance créée.'); }
      navigate('/maintenances');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      showError(msg || 'Erreur.');
    } finally { setSaving(false); }
  };

  return (
    <Box>
      <PageHeader
        title={isEdit ? 'Modifier la maintenance' : 'Nouvelle maintenance'}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Maintenances', to: '/maintenances' }, { label: isEdit ? 'Modifier' : 'Nouvelle' }]}
      />
      <Card>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <Controller name="equipementId" control={control} defaultValue="" render={({ field }) => (
                  <FormControl fullWidth required><InputLabel>Équipement *</InputLabel>
                    <Select {...field} label="Équipement *">
                      {equipements.map(e => <MenuItem key={e.id} value={e.id}>{e.nom} ({e.reference})</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={12} md={6}>
                <Controller name="technicienId" control={control} defaultValue="" render={({ field }) => (
                  <FormControl fullWidth required><InputLabel>Technicien *</InputLabel>
                    <Select {...field} label="Technicien *">
                      {techniciens.map(u => <MenuItem key={u.id} value={u.id}>{u.prenom} {u.nom}</MenuItem>)}
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6} md={3}>
                <Controller name="type" control={control} defaultValue="Preventive" render={({ field }) => (
                  <FormControl fullWidth><InputLabel>Type</InputLabel>
                    <Select {...field} label="Type">
                      <MenuItem value="Preventive">Préventive</MenuItem>
                      <MenuItem value="Corrective">Corrective</MenuItem>
                      <MenuItem value="Curative">Curative</MenuItem>
                    </Select>
                  </FormControl>
                )} />
              </Grid>
              <Grid item xs={6} md={3}><TextField {...register('datePlanifiee')} label="Date planifiée *" type="date" fullWidth required InputLabelProps={{ shrink: true }} /></Grid>
              <Grid item xs={6} md={3}><TextField {...register('dureeMinutes')} label="Durée (min)" type="number" fullWidth /></Grid>
              <Grid item xs={6} md={3}><TextField {...register('coutEstime')} label="Coût estimé (MAD)" type="number" fullWidth /></Grid>
              <Grid item xs={12}><TextField {...register('observations')} label="Observations" fullWidth multiline rows={3} /></Grid>
              {isEdit && <Grid item xs={12}><TextField {...register('piecesRemplacees')} label="Pièces remplacées" fullWidth /></Grid>}
              <Grid item xs={12} sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                <Button onClick={() => navigate('/maintenances')}>Annuler</Button>
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
