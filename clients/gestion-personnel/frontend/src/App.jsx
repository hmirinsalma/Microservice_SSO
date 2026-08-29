import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './routes/ProtectedRoute';
import MainLayout from './components/layout/MainLayout';
import LoginPage from './pages/auth/LoginPage';
import Callback from './pages/Callback';
import DashboardPage from './pages/dashboard/DashboardPage';
import EmployesPage from './pages/employes/EmployesPage';
import DirectionsPage from './pages/directions/DirectionsPage';
import ServicesPage from './pages/services/ServicesPage';
import ProfilPage from './pages/profil/ProfilPage';

import CongesPage from './pages/conges/CongesPage';
import UsersPage from './pages/users/UsersPage';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/callback" element={<Callback />} />
          <Route element={<ProtectedRoute />}>
            <Route element={<MainLayout />}>
              <Route path="/"               element={<DashboardPage />} />
              <Route path="/employes"       element={<EmployesPage />} />
              <Route path="/directions"     element={<DirectionsPage />} />
              <Route path="/services"       element={<ServicesPage />} />
              <Route path="/conges"         element={<CongesPage />} />
              <Route path="/utilisateurs"   element={<UsersPage />} />
              <Route path="/profil"         element={<ProfilPage />} />
            </Route>
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
