import React, { useEffect, useState, useCallback } from 'react';
import {
  Box, Button, Card, CardContent, TextField, MenuItem, Select, FormControl, InputLabel,
  Table, TableHead, TableRow, TableCell, TableBody, TablePagination,
  IconButton, Tooltip, InputAdornment, Grid, Typography, Chip, Avatar
} from '@mui/material';
import { IconPlus, IconEdit, IconTrash, IconEye, IconSearch, IconFilter, IconRefresh, IconPackage } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { getEquipements, deleteEquipement } from '../api/equipementsApi';
import { getCategories } from '../api/categoriesApi';
import { EquipementListDto, CategorieDto } from '../types';
import { useAuth } from '../contexts/AuthContext';
import { useSnackbar } from '../contexts/SnackbarContext';
import { useDebounce } from '../hooks/useDebounce';
import { canCreate, canDelete } from '../utils/roleGuard';
import { formatDate, formatCurrency, etatLabels } from '../utils/formatters';
import StatusBadge from '../components/common/StatusBadge';
import ConfirmDialog from '../components/common/ConfirmDialog';
import SkeletonTable from '../components/common/SkeletonTable';
import PageHeader from '../components/common/PageHeader';

export default function Equipements() {
  const { user } = useAuth();
  const { showSuccess, showError } = useSnackbar();
  const navigate = useNavigate();
  const [items, setItems] = useState<EquipementListDto[]>([]);
  const [categories, setCategories] = useState<CategorieDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState('');
  const [etatFilter, setEtatFilter] = useState('');
  const [categorieFilter, setCategorieFilter] = useState('');
  const [loading, setLoading] = useState(true);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const debouncedSearch = useDebounce(search, 300);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, unknown> = { page: page + 1, pageSize };
      if (debouncedSearch) params.search = debouncedSearch;
      if (etatFilter) params.etat = etatFilter;
      if (categorieFilter) params.categorieId = categorieFilter;
      const res = await getEquipements(params);
      setItems(res.data.data.items);
      setTotal(res.data.data.totalCount);
    } catch { showError('Erreur lors du chargement.'); }
    finally { setLoading(false); }
  }, [page, pageSize, debouncedSearch, etatFilter, categorieFilter]);

  useEffect(() => { load(); }, [load]);
  useEffect(() => { getCategories().then(r => setCategories(r.data.data)); }, []);

  const handleDelete = async () => {
    if (!deleteId) return;
    setDeleting(true);
    try { await deleteEquipement(deleteId); showSuccess('Équipement supprimé.'); setDeleteId(null); load(); }
    catch { showError('Erreur lors de la suppression.'); }
    finally { setDeleting(false); }
  };

  const reset = () => { setSearch(''); setEtatFilter(''); setCategorieFilter(''); };

  return (
    <Box className="fade-in">
      <PageHeader title="Équipements" subtitle={`${total} équipements trouvés`}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Équipements' }]}
        action={canCreate(user!.role) && (
          <Button variant="contained" startIcon={<IconPlus size={16} />}
            onClick={() => navigate('/equipements/nouveau')}
            sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)', boxShadow: '0 4px 14px rgba(0,102,204,0.3)' }}>
            Nouvel équipement
          </Button>
        )}
      />

      {/* Filters */}
      <Card sx={{ mb: 2.5 }}>
        <CardContent sx={{ p: 2 }}>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} md={4}>
              <TextField fullWidth size="small" placeholder="Nom, référence, numéro de série..."
                value={search} onChange={(e) => setSearch(e.target.value)}
                InputProps={{ startAdornment: <InputAdornment position="start"><IconSearch size={16} color="#A0AEC0" /></InputAdornment> }}
              />
            </Grid>
            <Grid item xs={6} md={2.5}>
              <FormControl fullWidth size="small">
                <InputLabel>État</InputLabel>
                <Select value={etatFilter} onChange={(e) => setEtatFilter(e.target.value)} label="État">
                  <MenuItem value="">Tous les états</MenuItem>
                  {Object.entries(etatLabels).map(([v, l]) => <MenuItem key={v} value={v}>{l}</MenuItem>)}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6} md={2.5}>
              <FormControl fullWidth size="small">
                <InputLabel>Catégorie</InputLabel>
                <Select value={categorieFilter} onChange={(e) => setCategorieFilter(e.target.value)} label="Catégorie">
                  <MenuItem value="">Toutes</MenuItem>
                  {categories.map(c => <MenuItem key={c.id} value={c.id}>{c.nom}</MenuItem>)}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={3} sx={{ display: 'flex', gap: 1 }}>
              <Button startIcon={<IconRefresh size={15} />} onClick={reset} size="small" variant="outlined" sx={{ borderColor: '#E2E8F0', color: '#718096' }}>
                Réinitialiser
              </Button>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Table */}
      <Card>
        {loading ? <SkeletonTable /> : items.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <IconPackage size={48} color="#CBD5E0" />
            <Typography variant="h6" color="text.secondary" sx={{ mt: 2 }}>Aucun équipement trouvé</Typography>
            <Typography variant="body2" color="text.secondary">Modifiez vos filtres ou créez un nouvel équipement.</Typography>
          </Box>
        ) : (
          <>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Référence</TableCell>
                  <TableCell>Équipement</TableCell>
                  <TableCell>Catégorie</TableCell>
                  <TableCell>Localisation</TableCell>
                  <TableCell>Service</TableCell>
                  <TableCell>État</TableCell>
                  <TableCell>Installation</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map(eq => (
                  <TableRow key={eq.id}>
                    <TableCell>
                      <Typography sx={{ fontFamily: 'monospace', fontSize: '0.8rem', fontWeight: 600, color: '#0066CC', bgcolor: '#EBF8FF', px: 1, py: 0.3, borderRadius: '6px', display: 'inline' }}>
                        {eq.reference}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                        <Avatar sx={{ width: 32, height: 32, bgcolor: `${eq.couleurCategorie}20`, color: eq.couleurCategorie, fontSize: '0.75rem', fontWeight: 800 }}>
                          {eq.categorieCode.substring(0, 2)}
                        </Avatar>
                        <Box>
                          <Typography sx={{ fontWeight: 600, fontSize: '0.875rem', lineHeight: 1.3 }}>{eq.nom}</Typography>
                          <Typography sx={{ fontSize: '0.75rem', color: '#A0AEC0' }}>{eq.marque} · {eq.modele}</Typography>
                        </Box>
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Chip label={eq.categorieName} size="small" sx={{ bgcolor: `${eq.couleurCategorie}15`, color: eq.couleurCategorie, border: `1px solid ${eq.couleurCategorie}30`, fontWeight: 600, fontSize: '0.72rem' }} />
                    </TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{eq.localisation}</Typography></TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{eq.serviceNom}</Typography></TableCell>
                    <TableCell><StatusBadge etat={eq.etat} /></TableCell>
                    <TableCell><Typography sx={{ fontSize: '0.82rem', color: '#718096' }}>{formatDate(eq.dateInstallation)}</Typography></TableCell>
                    <TableCell align="right">
                      <Box sx={{ display: 'flex', gap: 0.5, justifyContent: 'flex-end' }}>
                        <Tooltip title="Voir le détail">
                          <IconButton size="small" onClick={() => navigate(`/equipements/${eq.id}`)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#EBF8FF', color: '#0066CC' } }}>
                            <IconEye size={16} />
                          </IconButton>
                        </Tooltip>
                        {canCreate(user!.role) && (
                          <Tooltip title="Modifier">
                            <IconButton size="small" onClick={() => navigate(`/equipements/${eq.id}/modifier`)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#F0FFF4', color: '#00A86B' } }}>
                              <IconEdit size={16} />
                            </IconButton>
                          </Tooltip>
                        )}
                        {canDelete(user!.role) && (
                          <Tooltip title="Supprimer">
                            <IconButton size="small" onClick={() => setDeleteId(eq.id)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#FFF5F5', color: '#E53E3E' } }}>
                              <IconTrash size={16} />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Box>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
            <Box sx={{ borderTop: '1px solid #F0F4F8' }}>
              <TablePagination component="div" count={total} page={page} rowsPerPage={pageSize}
                onPageChange={(_, p) => setPage(p)} onRowsPerPageChange={(e) => { setPageSize(+e.target.value); setPage(0); }}
                rowsPerPageOptions={[10, 20, 50]} labelRowsPerPage="Par page"
                sx={{ '& .MuiTablePagination-select': { fontWeight: 600 } }}
              />
            </Box>
          </>
        )}
      </Card>
      <ConfirmDialog open={!!deleteId} title="Supprimer l'équipement"
        message="Cette action est irréversible. L'équipement et tout son historique seront supprimés."
        onConfirm={handleDelete} onCancel={() => setDeleteId(null)} loading={deleting} />
    </Box>
  );
}
