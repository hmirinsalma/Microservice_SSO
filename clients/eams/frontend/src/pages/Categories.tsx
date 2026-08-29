import React, { useEffect, useState } from 'react';
import {
  Box, Button, Card, CardContent, Grid, TextField, Dialog, DialogTitle,
  DialogContent, DialogActions, IconButton, Tooltip, Typography, Chip, Divider
} from '@mui/material';
import { IconPlus, IconEdit, IconTrash, IconCategory, IconPalette } from '@tabler/icons-react';
import { getCategories, createCategorie, updateCategorie, deleteCategorie } from '../api/categoriesApi';
import { CategorieDto } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import { useForm } from 'react-hook-form';
import PageHeader from '../components/common/PageHeader';
import ConfirmDialog from '../components/common/ConfirmDialog';
import SkeletonTable from '../components/common/SkeletonTable';

type FormData = { nom: string; description: string; icone: string; couleur: string; code: string };

export default function Categories() {
  const { showSuccess, showError } = useSnackbar();
  const [items, setItems] = useState<CategorieDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<CategorieDto | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const { register, handleSubmit, reset } = useForm<FormData>();

  const load = () => getCategories().then(r => setItems(r.data.data)).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const openCreate = () => {
    setEditing(null);
    reset({ nom: '', description: '', icone: 'Build', couleur: '#0066CC', code: '' });
    setOpen(true);
  };
  const openEdit = (c: CategorieDto) => {
    setEditing(c);
    reset({ nom: c.nom, description: c.description, icone: c.icone, couleur: c.couleur, code: c.code });
    setOpen(true);
  };

  const onSubmit = async (d: FormData) => {
    try {
      if (editing) { await updateCategorie(editing.id, d); showSuccess('Catégorie mise à jour.'); }
      else { await createCategorie(d); showSuccess('Catégorie créée.'); }
      setOpen(false); load();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      showError(msg || 'Erreur.');
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    setDeleting(true);
    try { await deleteCategorie(deleteId); showSuccess('Catégorie supprimée.'); setDeleteId(null); load(); }
    catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      showError(msg || 'Erreur lors de la suppression.');
    }
    finally { setDeleting(false); }
  };

  return (
    <Box className="fade-in">
      <PageHeader title="Catégories" subtitle={`${items.length} catégories configurées`}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Catégories' }]}
        action={
          <Button variant="contained" startIcon={<IconPlus size={16} />} onClick={openCreate}
            sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)', boxShadow: '0 4px 14px rgba(0,102,204,0.3)' }}>
            Nouvelle catégorie
          </Button>
        }
      />

      {loading ? <SkeletonTable /> : (
        <Grid container spacing={2.5}>
          {items.map(c => (
            <Grid item xs={12} sm={6} md={4} key={c.id}>
              <Card sx={{ '&:hover': { transform: 'translateY(-2px)' }, transition: 'all 0.2s' }}>
                <CardContent sx={{ p: 2.5 }}>
                  <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 2 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                      <Box sx={{
                        width: 44, height: 44, borderRadius: '12px', flexShrink: 0,
                        background: `${c.couleur}20`, border: `2px solid ${c.couleur}40`,
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                      }}>
                        <IconPalette size={20} color={c.couleur} />
                      </Box>
                      <Box>
                        <Typography sx={{ fontWeight: 700, fontSize: '0.95rem', lineHeight: 1.2 }}>{c.nom}</Typography>
                        <Typography sx={{ fontSize: '0.72rem', color: '#A0AEC0', fontFamily: 'monospace', fontWeight: 700 }}>{c.code}</Typography>
                      </Box>
                    </Box>
                    <Box sx={{ display: 'flex', gap: 0.5 }}>
                      <Tooltip title="Modifier">
                        <IconButton size="small" onClick={() => openEdit(c)} sx={{ color: '#718096', '&:hover': { bgcolor: '#EBF8FF', color: '#0066CC' } }}>
                          <IconEdit size={15} />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Supprimer">
                        <IconButton size="small" onClick={() => setDeleteId(c.id)} sx={{ color: '#718096', '&:hover': { bgcolor: '#FFF5F5', color: '#E53E3E' } }}>
                          <IconTrash size={15} />
                        </IconButton>
                      </Tooltip>
                    </Box>
                  </Box>

                  <Typography variant="body2" color="text.secondary" sx={{ mb: 2, fontSize: '0.82rem', lineHeight: 1.5, minHeight: 36 }}>
                    {c.description || 'Aucune description.'}
                  </Typography>

                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: c.couleur }} />
                      <Typography sx={{ fontSize: '0.75rem', color: '#718096', fontFamily: 'monospace' }}>{c.couleur}</Typography>
                    </Box>
                    <Chip
                      label={`${c.nbEquipements} équipements`}
                      size="small"
                      sx={{ bgcolor: `${c.couleur}15`, color: c.couleur, border: `1px solid ${c.couleur}30`, fontWeight: 700, fontSize: '0.72rem' }}
                    />
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {/* Dialog create/edit */}
      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth
        PaperProps={{ sx: { borderRadius: '16px' } }}>
        <DialogTitle sx={{ pb: 1, fontWeight: 700 }}>
          {editing ? 'Modifier la catégorie' : 'Nouvelle catégorie'}
        </DialogTitle>
        <Divider />
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent sx={{ pt: 2.5 }}>
            <Grid container spacing={2}>
              <Grid item xs={8}>
                <TextField {...register('nom')} label="Nom *" fullWidth size="small" required />
              </Grid>
              <Grid item xs={4}>
                <TextField {...register('code')} label="Code *" fullWidth size="small" required inputProps={{ maxLength: 5 }}
                  helperText="Ex: TRF, CPT" />
              </Grid>
              <Grid item xs={12}>
                <TextField {...register('description')} label="Description" fullWidth size="small" multiline rows={2} />
              </Grid>
              <Grid item xs={8}>
                <TextField {...register('icone')} label="Icône (nom MUI)" fullWidth size="small" placeholder="Build, ElectricMeter..." />
              </Grid>
              <Grid item xs={4}>
                <TextField {...register('couleur')} label="Couleur hex" fullWidth size="small" placeholder="#0066CC" />
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

      <ConfirmDialog open={!!deleteId} title="Supprimer la catégorie"
        message="Cette catégorie sera supprimée si aucun équipement n'y est associé. Cette action est irréversible."
        onConfirm={handleDelete} onCancel={() => setDeleteId(null)} loading={deleting} />
    </Box>
  );
}
