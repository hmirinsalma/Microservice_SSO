import Badge from './Badge';

const MAP = {
  EnAttente:       { variant: 'warning',  label: 'En attente' },
  ValideChef:      { variant: 'info',     label: 'Validé chef' },
  ValideDirecteur: { variant: 'success',  label: 'Approuvé' },
  Refuse:          { variant: 'danger',   label: 'Refusé' },
  Annule:          { variant: 'neutral',  label: 'Annulé' },
};

export default function CongeBadge({ statut }) {
  const cfg = MAP[statut] || { variant: 'neutral', label: statut };
  return <Badge variant={cfg.variant}>{cfg.label}</Badge>;
}
