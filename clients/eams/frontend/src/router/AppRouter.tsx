import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import AppLayout from '../components/layout/AppLayout';
import ProtectedRoute from './ProtectedRoute';
import LoginSSO from '../pages/LoginSSO';
import Callback from '../pages/Callback';
import Dashboard from '../pages/Dashboard';
import Equipements from '../pages/Equipements';
import EquipementDetail from '../pages/EquipementDetail';
import EquipementForm from '../pages/EquipementForm';
import Maintenances from '../pages/Maintenances';
import MaintenanceForm from '../pages/MaintenanceForm';
import Notifications from '../pages/Notifications';
import MonProfil from '../pages/MonProfil';
import Categories from '../pages/Categories';
import Utilisateurs from '../pages/Utilisateurs';

export default function AppRouter() {
  const { isAuthenticated } = useAuth();

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={isAuthenticated ? <Navigate to="/" replace /> : <LoginSSO />} />
        <Route path="/auth/callback" element={<Callback />} />
        <Route element={<ProtectedRoute><AppLayout /></ProtectedRoute>}>
          <Route path="/" element={<Dashboard />} />
          <Route path="/equipements" element={<Equipements />} />
          <Route path="/equipements/:id" element={<EquipementDetail />} />
          <Route path="/equipements/nouveau" element={
            <ProtectedRoute roles={['Admin_Patrimoine']}><EquipementForm /></ProtectedRoute>
          } />
          <Route path="/equipements/:id/modifier" element={
            <ProtectedRoute roles={['Admin_Patrimoine']}><EquipementForm /></ProtectedRoute>
          } />
          <Route path="/maintenances" element={<Maintenances />} />
          <Route path="/maintenances/:id" element={<MaintenanceForm />} />
          <Route path="/maintenances/nouvelle" element={
            <ProtectedRoute roles={['Admin_Patrimoine', 'Chef_de_Service']}><MaintenanceForm /></ProtectedRoute>
          } />
          <Route path="/maintenances/:id/modifier" element={<MaintenanceForm />} />
          <Route path="/notifications" element={<Notifications />} />
          <Route path="/profil" element={<MonProfil />} />
          <Route path="/categories" element={
            <ProtectedRoute roles={['Admin_Patrimoine']}><Categories /></ProtectedRoute>
          } />
          <Route path="/utilisateurs" element={
            <ProtectedRoute roles={['Admin_Patrimoine']}><Utilisateurs /></ProtectedRoute>
          } />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
