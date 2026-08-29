export const ROLES = {
  ADMIN:     'Administrateur_Technique',
  DIRECTEUR: 'Directeur_Technique',
  CHEF:      'Chef_de_Service',
  TECH:      'Technicien',
}

export const STATUS_CONFIG = {
  Nouvelle:  { label: 'Nouvelle',   color: 'info',    bgColor: '#E3F2FD', textColor: '#1565C0' },
  EnCours:   { label: 'En cours',   color: 'warning', bgColor: '#FFF3E0', textColor: '#E65100' },
  Suspendue: { label: 'Suspendue',  color: 'default', bgColor: '#F3E5F5', textColor: '#6A1B9A' },
  Terminee:  { label: 'Terminée',   color: 'success', bgColor: '#E8F5E9', textColor: '#1B5E20' },
  Annulee:   { label: 'Annulée',    color: 'error',   bgColor: '#FFEBEE', textColor: '#B71C1C' },
}

export const PRIORITY_CONFIG = {
  Faible:   { label: 'Faible',    color: '#757575', bgColor: '#F5F5F5', chip: 'default'  },
  Normale:  { label: 'Normale',   color: '#1565C0', bgColor: '#E3F2FD', chip: 'primary'  },
  Urgente:  { label: 'Urgente',   color: '#E65100', bgColor: '#FFF3E0', chip: 'warning'  },
  Critique: { label: 'Critique',  color: '#B71C1C', bgColor: '#FFEBEE', chip: 'error'    },
}

export const ACTION_LABELS = {
  Creation:              'Création',
  Modification:          'Modification',
  ChangementTechnicien:  'Changement de technicien',
  ChangementResponsable: 'Changement de responsable',
  ChangementStatut:      'Changement de statut',
  ChangementPriorite:    'Changement de priorité',
  AjoutCommentaire:      'Ajout de commentaire',
  AjoutPieceJointe:      'Ajout de pièce jointe',
  Affectation:           'Affectation',
  RetraitAffectation:    'Retrait d\'affectation',
  AjoutCompteRendu:      'Compte rendu',
}
