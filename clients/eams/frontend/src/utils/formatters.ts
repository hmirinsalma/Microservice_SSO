export const formatDate = (d?: string | null) =>
  d ? new Date(d).toLocaleDateString('fr-FR') : '—';

export const formatDateTime = (d?: string | null) =>
  d ? new Date(d).toLocaleString('fr-FR') : '—';

export const formatCurrency = (v?: number | null) =>
  v != null ? new Intl.NumberFormat('fr-MA', { style: 'currency', currency: 'MAD' }).format(v) : '—';

export const etatLabels: Record<string, string> = {
  Disponible: 'Disponible',
  En_maintenance: 'En maintenance',
  En_panne: 'En panne',
  Hors_service: 'Hors service',
  Reserve: 'Réservé',
};

export const typeMaintenanceLabels: Record<string, string> = {
  Preventive: 'Préventive',
  Corrective: 'Corrective',
  Curative: 'Curative',
};

export const statutLabels: Record<string, string> = {
  Planifiee: 'Planifiée',
  En_cours: 'En cours',
  Terminee: 'Terminée',
  Annulee: 'Annulée',
  En_retard: 'En retard',
};
