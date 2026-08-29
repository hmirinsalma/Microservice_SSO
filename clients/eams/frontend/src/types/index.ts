export type UserRole = 'Admin_Patrimoine' | 'Directeur' | 'Chef_de_Service' | 'Technicien';

export type EquipementEtat = 'Disponible' | 'En_maintenance' | 'En_panne' | 'Hors_service' | 'Reserve';

export type MaintenanceType = 'Preventive' | 'Corrective' | 'Curative';

export type MaintenanceStatut = 'Planifiee' | 'En_cours' | 'Terminee' | 'Annulee' | 'En_retard';

export interface AuthUser {
  id: string;
  nom: string;
  prenom: string;
  email: string;
  role: UserRole;
  serviceId?: string;
  token: string;
  expiresAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  error?: { message: string; details: string[] };
  statusCode: number;
  timestamp: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  totalPages: number;
  page: number;
  pageSize: number;
}

export interface CategorieDto {
  id: string;
  nom: string;
  description: string;
  icone: string;
  couleur: string;
  code: string;
  nbEquipements: number;
}

export interface EquipementListDto {
  id: string;
  reference: string;
  nom: string;
  categorieName: string;
  categorieCode: string;
  couleurCategorie: string;
  type: string;
  marque: string;
  modele: string;
  numeroSerie: string;
  localisation: string;
  serviceNom: string;
  responsableNom: string;
  dateInstallation: string;
  etat: EquipementEtat;
  dateFinGarantie?: string;
  valeurAcquisition?: number;
}

export interface EquipementDetailDto extends EquipementListDto {
  categorieId: string;
  iconeCategorie: string;
  serviceId: string;
  responsableId: string;
  dateMiseEnService?: string;
  fournisseur?: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
  documents: DocumentDto[];
  photos: PhotoDto[];
}

export interface DocumentDto {
  id: string;
  nomFichier: string;
  url: string;
  extension: string;
  tailleOctets: number;
  uploadedAt: string;
}

export interface PhotoDto {
  id: string;
  url: string;
  isMain: boolean;
  uploadedAt: string;
}

export interface MaintenanceListDto {
  id: string;
  equipementId: string;
  equipementNom: string;
  equipementReference: string;
  technicienId: string;
  technicienNom: string;
  type: MaintenanceType;
  statut: MaintenanceStatut;
  datePlanifiee: string;
  coutEstime?: number;
  createdAt: string;
}

export interface MaintenanceDetailDto extends MaintenanceListDto {
  dateDebut?: string;
  dateCloture?: string;
  dureeMinutes?: number;
  etatAvant?: EquipementEtat;
  etatApres?: EquipementEtat;
  observations?: string;
  piecesRemplacees?: string;
  coutReel?: number;
  prochaineMaintenance?: string;
  updatedAt: string;
}

export interface NotificationDto {
  id: string;
  typeEvenement: string;
  message: string;
  ressourceId: string;
  ressourceType: string;
  estLue: boolean;
  createdAt: string;
}

export interface UserDto {
  id: string;
  nom: string;
  prenom: string;
  email: string;
  telephone: string;
  poste: string;
  photoUrl?: string;
  role: UserRole;
  serviceId?: string;
  serviceNom?: string;
  isActive: boolean;
}

export interface HistoriqueEntryDto {
  id: string;
  entiteType: string;
  typeEvenement: string;
  valeurAvant?: string;
  valeurApres?: string;
  auteurNom: string;
  horodatageUtc: string;
}
