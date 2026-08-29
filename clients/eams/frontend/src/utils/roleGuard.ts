import { UserRole } from '../types';

export const canCreate = (role: UserRole) => role === 'Admin_Patrimoine';
export const canDelete = (role: UserRole) => role === 'Admin_Patrimoine';
export const canManageUsers = (role: UserRole) => role === 'Admin_Patrimoine';
export const canManageCategories = (role: UserRole) => role === 'Admin_Patrimoine';
export const canCreateMaintenance = (role: UserRole) =>
  role === 'Admin_Patrimoine' || role === 'Chef_de_Service';
export const canViewAllServices = (role: UserRole) =>
  role === 'Admin_Patrimoine' || role === 'Directeur';

export const getSidebarItems = (role: UserRole) => {
  const items = [
    { label: 'Tableau de bord', path: '/', icon: 'Dashboard' },
    { label: 'Équipements', path: '/equipements', icon: 'Build' },
    { label: 'Maintenances', path: '/maintenances', icon: 'Engineering' },
    { label: 'Notifications', path: '/notifications', icon: 'Notifications' },
    { label: 'Mon Profil', path: '/profil', icon: 'Person' },
  ];
  if (role === 'Admin_Patrimoine') {
    items.splice(3, 0,
      { label: 'Catégories', path: '/categories', icon: 'Category' },
      { label: 'Utilisateurs', path: '/utilisateurs', icon: 'Group' }
    );
  }
  return items;
};
