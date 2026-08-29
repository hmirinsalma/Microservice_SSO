import React, { useEffect, useState } from 'react';
import { Box, Card, List, ListItem, ListItemText, ListItemIcon, IconButton, Button, Typography, Divider, Tooltip } from '@mui/material';
import { IconBell, IconBellRinging, IconCheck, IconChecks, IconAlertTriangle, IconTool, IconBuildingFactory2, IconCalendar, IconUser } from '@tabler/icons-react';
import { getNotifications, markAsRead, markAllAsRead } from '../api/notificationsApi';
import { NotificationDto } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';
import { formatDateTime } from '../utils/formatters';

const typeConfig: Record<string, { icon: React.ReactNode; color: string; bg: string; label: string }> = {
  MaintenancePlanifiee: { icon: <IconCalendar size={18} />, color: '#2C5282', bg: '#EBF8FF', label: 'Maintenance planifiée' },
  MaintenanceEnRetard:  { icon: <IconAlertTriangle size={18} />, color: '#9B2C2C', bg: '#FFF5F5', label: 'Maintenance en retard' },
  EquipementEnPanne:    { icon: <IconBuildingFactory2 size={18} />, color: '#9B2C2C', bg: '#FFF5F5', label: 'Équipement en panne' },
  EquipementRemisEnService: { icon: <IconTool size={18} />, color: '#276749', bg: '#F0FFF4', label: 'Remis en service' },
  GarantieExpirante:    { icon: <IconAlertTriangle size={18} />, color: '#9C4221', bg: '#FFFAF0', label: 'Garantie expirante' },
  NouvelleAffectation:  { icon: <IconUser size={18} />, color: '#553C9A', bg: '#FAF5FF', label: 'Nouvelle affectation' },
};

export default function Notifications() {
  const { showSuccess, showError } = useSnackbar();
  const [items, setItems] = useState<NotificationDto[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => getNotifications().then(r => setItems(r.data.data)).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const handleRead = async (id: string) => {
    try { await markAsRead(id); load(); } catch { showError('Erreur.'); }
  };

  const handleReadAll = async () => {
    try { await markAllAsRead(); showSuccess('Toutes les notifications marquées comme lues.'); load(); }
    catch { showError('Erreur.'); }
  };

  const unread = items.filter(n => !n.estLue).length;

  return (
    <Box className="fade-in">
      <PageHeader title="Notifications" subtitle={unread > 0 ? `${unread} non lues` : 'Tout est à jour'}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Notifications' }]}
        action={unread > 0 && (
          <Button startIcon={<IconChecks size={16} />} onClick={handleReadAll} variant="outlined"
            sx={{ borderColor: '#E2E8F0', color: '#718096' }}>
            Tout marquer lu
          </Button>
        )}
      />

      <Card>
        {loading ? <SkeletonTable rows={6} /> : items.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <IconBell size={48} color="#CBD5E0" />
            <Typography variant="h6" color="text.secondary" sx={{ mt: 2 }}>Aucune notification</Typography>
            <Typography variant="body2" color="text.secondary">Vous êtes à jour !</Typography>
          </Box>
        ) : (
          <List disablePadding>
            {items.map((n, i) => {
              const cfg = typeConfig[n.typeEvenement] || { icon: <IconBellRinging size={18} />, color: '#4A5568', bg: '#F7FAFC', label: n.typeEvenement };
              return (
                <React.Fragment key={n.id}>
                  <ListItem
                    sx={{
                      px: 3, py: 1.8,
                      bgcolor: n.estLue ? 'transparent' : `${cfg.bg}`,
                      borderLeft: n.estLue ? '3px solid transparent' : `3px solid ${cfg.color}`,
                      transition: 'all 0.2s',
                      '&:hover': { bgcolor: '#F7FAFC' },
                    }}
                    secondaryAction={!n.estLue && (
                      <Tooltip title="Marquer comme lu">
                        <IconButton size="small" onClick={() => handleRead(n.id)}
                          sx={{ color: cfg.color, '&:hover': { bgcolor: `${cfg.bg}` } }}>
                          <IconCheck size={16} />
                        </IconButton>
                      </Tooltip>
                    )}
                  >
                    <ListItemIcon sx={{ minWidth: 44 }}>
                      <Box sx={{ width: 36, height: 36, borderRadius: '10px', bgcolor: cfg.bg, color: cfg.color, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        {cfg.icon}
                      </Box>
                    </ListItemIcon>
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                          <Typography sx={{ fontSize: '0.875rem', fontWeight: n.estLue ? 400 : 600, color: '#1A202C' }}>
                            {n.message}
                          </Typography>
                          {!n.estLue && (
                            <Box component="span" sx={{ width: 7, height: 7, borderRadius: '50%', bgcolor: cfg.color, flexShrink: 0 }} />
                          )}
                        </Box>
                      }
                      secondary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mt: 0.5 }}>
                          <Box component="span" sx={{ fontSize: '0.72rem', fontWeight: 600, px: 1, py: 0.2, borderRadius: '5px', bgcolor: cfg.bg, color: cfg.color }}>
                            {cfg.label}
                          </Box>
                          <Typography component="span" sx={{ fontSize: '0.75rem', color: '#A0AEC0' }}>
                            {formatDateTime(n.createdAt)}
                          </Typography>
                        </Box>
                      }
                    />
                  </ListItem>
                  {i < items.length - 1 && <Divider sx={{ borderColor: '#F0F4F8' }} />}
                </React.Fragment>
              );
            })}
          </List>
        )}
      </Card>
    </Box>
  );
}
