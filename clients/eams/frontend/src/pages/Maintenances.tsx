import React, { useEffect, useState, useCallback } from 'react';
import {
  Box, Button, Card, CardContent, TextField, MenuItem, Select, FormControl, InputLabel,
  Table, TableHead, TableRow, TableCell, TableBody, TablePagination,
  IconButton, Tooltip, Grid, Typography, Chip, Avatar
} from '@mui/material';
import { IconPlus, IconEdit, IconTrash, IconEye, IconRefresh, IconEngine } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { getMaintenances, deleteMaintenance } from '../api/maintenancesApi';
import { MaintenanceListDto } from '../types';
import { useAuth } from '../contexts/AuthContext';
import { useSnackbar } from '../contexts/SnackbarContext';
import { canCreateMaintenance, canDelete } from '../utils/roleGuard';
import { formatDate, formatCurrency, typeMaintenanceLabels, statutLabels } from '../utils/formatters';
import ConfirmDialog from '../components/common/ConfirmDialog';
import SkeletonTable from '../components/common/SkeletonTable';
import PageHeader from '../components/common/PageHeader';

const statutStyles: Record<string, { bg: string; color: string; border: string }> = {
  Planifiee:  { bg: '#EBF8FF', color: '#2C5282', border: '#90CDF4' },
  En_cours:   { bg: '#FFFAF0', color: '#9C4221', border: '#FBD38D' },
  Terminee:   { bg: '#F0FFF4', color: '#276749', border: '#9AE6B4' },
  Annulee:    { bg: '#F7FAFC', color: '#4A5568', border: '#CBD5E0' },
  En_retard:  { bg: '#FFF5F5', color: '#9B2C2C', border: '#FEB2B2' },
};

const typeStyles: Record<string, { bg: string; color: string }> = {
  Preventive: { bg: '#EBF8FF', color: '#2C5282' },
  Corrective: { bg: '#FFFAF0', color: '#9C4221' },
  Curative:   { bg: '#FFF5F5', color: '#9B2C2C' },
};

export default function Maintenances() {
  const { user } = useAuth();
  const { showSuccess, showError } = useSnackbar();
  const navigate = useNavigate();
  const [items, setItems] = useState<MaintenanceListDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [statutFilter, setStatutFilter] = useState('');
  const [typeFilter, setTypeFilter] = useState('');
  const [loading, setLoading] = useState(true);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, unknown> = { page: page + 1, pageSize };
      if (statutFilter) params.statut = statutFilter;
      if (typeFilter)   params.type   = typeFilter;
      const res = await getMaintenances(params);
      setItems(res.data.data.items);
      setTotal(res.data.data.totalCount);
    } catch { showError('Erreur chargement maintenances.'); }
    finally { setLoading(false); }
  }, [page, pageSize, statutFilter, typeFilter]);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async () => {
    if (!deleteId) return;
    setDeleting(true);
    try { await deleteMaintenance(deleteId); showSuccess('Maintenance supprimée.'); setDeleteId(null); load(); }
    catch { showError('Erreur lors de la suppression.'); }
    finally { setDeleting(false); }
  };

  return (
    <Box className="fade-in">
      <PageHeader title="Maintenances" subtitle={`${total} interventions au total`}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Maintenances' }]}
        action={canCreateMaintenance(user!.role) && (
          <Button variant="contained" startIcon={<IconPlus size={16} />}
            onClick={() => navigate('/maintenances/nouvelle')}
            sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)', boxShadow: '0 4px 14px rgba(0,102,204,0.3)' }}>
            Nouvelle maintenance
          </Button>
        )}
      />

      <Card sx={{ mb: 2.5 }}>
        <CardContent sx={{ p: 2 }}>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={6} md={3}>
              <FormControl fullWidth size="small">
                <InputLabel>Statut</InputLabel>
                <Select value={statutFilter} onChange={(e) => setStatutFilter(e.target.value)} label="Statut">
                  <MenuItem value="">Tous les statuts</MenuItem>
                  {Object.entries(statutLabels).map(([v, l]) => <MenuItem key={v} value={v}>{l}</MenuItem>)}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6} md={3}>
              <FormControl fullWidth size="small">
                <InputLabel>Type</InputLabel>
                <Select value={typeFilter} onChange={(e) => setTypeFilter(e.target.value)} label="Type">
                  <MenuItem value="">Tous les types</MenuItem>
                  {Object.entries(typeMaintenanceLabels).map(([v, l]) => <MenuItem key={v} value={v}>{l}</MenuItem>)}
                </Select>
              </FormControl>
            </Grid>
            <Grid item>
              <Button startIcon={<IconRefresh size={15} />}
                onClick={() => { setStatutFilter(''); setTypeFilter(''); }} size="small"
                variant="outlined" sx={{ borderColor: '#E2E8F0', color: '#718096' }}>
                Réinitialiser
              </Button>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      <Card>
        {loading ? <SkeletonTable /> : items.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <IconEngine size={48} color="#CBD5E0" />
            <Typography variant="h6" color="text.secondary" sx={{ mt: 2 }}>Aucune maintenance trouvée</Typography>
          </Box>
        ) : (
          <>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Équipement</TableCell>
                  <TableCell>Technicien</TableCell>
                  <TableCell>Type</TableCell>
                  <TableCell>Statut</TableCell>
                  <TableCell>Date planifiée</TableCell>
                  <TableCell>Coût estimé</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map(m => {
                  const ss = statutStyles[m.statut] || statutStyles.Planifiee;
                  const ts = typeStyles[m.type] || typeStyles.Preventive;
                  return (
                    <TableRow key={m.id}>
                      <TableCell>
                        <Box>
                          <Typography sx={{ fontWeight: 600, fontSize: '0.875rem' }}>{m.equipementNom}</Typography>
                          <Typography sx={{ fontSize: '0.75rem', color: '#A0AEC0', fontFamily: 'monospace' }}>{m.equipementReference}</Typography>
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Avatar sx={{ width: 28, height: 28, fontSize: '0.72rem', fontWeight: 700, bgcolor: '#EBF8FF', color: '#2C5282' }}>
                            {m.technicienNom.split(' ').map(n => n[0]).join('').substring(0, 2)}
                          </Avatar>
                          <Typography sx={{ fontSize: '0.85rem' }}>{m.technicienNom}</Typography>
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Box component="span" sx={{ display: 'inline-block', px: 1.2, py: 0.3, borderRadius: '6px', fontSize: '0.72rem', fontWeight: 700, bgcolor: ts.bg, color: ts.color }}>
                          {typeMaintenanceLabels[m.type]}
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Box component="span" sx={{ display: 'inline-flex', alignItems: 'center', gap: '5px', px: 1.2, py: 0.4, borderRadius: '20px', fontSize: '0.72rem', fontWeight: 600, bgcolor: ss.bg, color: ss.color, border: `1px solid ${ss.border}` }}>
                          <Box component="span" sx={{ width: 6, height: 6, borderRadius: '50%', bgcolor: ss.color }} />
                          {statutLabels[m.statut]}
                        </Box>
                      </TableCell>
                      <TableCell><Typography sx={{ fontSize: '0.85rem', color: '#4A5568' }}>{formatDate(m.datePlanifiee)}</Typography></TableCell>
                      <TableCell><Typography sx={{ fontSize: '0.85rem', fontWeight: 600, color: '#0066CC' }}>{formatCurrency(m.coutEstime)}</Typography></TableCell>
                      <TableCell align="right">
                        <Box sx={{ display: 'flex', gap: 0.5, justifyContent: 'flex-end' }}>
                          <Tooltip title="Modifier">
                            <IconButton size="small" onClick={() => navigate(`/maintenances/${m.id}/modifier`)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#F0FFF4', color: '#00A86B' } }}>
                              <IconEdit size={16} />
                            </IconButton>
                          </Tooltip>
                          {canDelete(user!.role) && (
                            <Tooltip title="Supprimer">
                              <IconButton size="small" onClick={() => setDeleteId(m.id)} sx={{ color: '#4A5568', '&:hover': { bgcolor: '#FFF5F5', color: '#E53E3E' } }}>
                                <IconTrash size={16} />
                              </IconButton>
                            </Tooltip>
                          )}
                        </Box>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
            <Box sx={{ borderTop: '1px solid #F0F4F8' }}>
              <TablePagination component="div" count={total} page={page} rowsPerPage={pageSize}
                onPageChange={(_, p) => setPage(p)} onRowsPerPageChange={(e) => { setPageSize(+e.target.value); setPage(0); }}
                rowsPerPageOptions={[10, 20, 50]} labelRowsPerPage="Par page" />
            </Box>
          </>
        )}
      </Card>
      <ConfirmDialog open={!!deleteId} title="Supprimer la maintenance" message="Cette action est irréversible."
        onConfirm={handleDelete} onCancel={() => setDeleteId(null)} loading={deleting} />
    </Box>
  );
}
