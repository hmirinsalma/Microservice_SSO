import React from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { ThemeProvider, CssBaseline } from '@mui/material'
import theme from './theme'
import { AuthProvider, useAuth } from './context/AuthContext'
import { SnackbarProvider } from './context/SnackbarContext'
import AppLayout from './components/layout/AppLayout'
import LoginPage from './pages/auth/LoginPage'
import LoginSSO from './pages/auth/LoginSSO'
import Callback from './pages/auth/Callback'
import DashboardPage from './pages/dashboard/DashboardPage'
import DashboardSSO from './pages/dashboard/DashboardSSO'
import InterventionsPage from './pages/interventions/InterventionsPage'
import InterventionFormPage from './pages/interventions/InterventionFormPage'
import InterventionDetailPage from './pages/interventions/InterventionDetailPage'
import UsersPage from './pages/users/UsersPage'
import ProfilePage from './pages/profile/ProfilePage'
import ServicesPage from './pages/services/ServicesPage'
import TechniciensPage from './pages/techniciens/TechniciensPage'
import HistoriquePage from './pages/historique/HistoriquePage'
import NotificationsPage from './pages/notifications/NotificationsPage'
import { ROLES } from './utils/constants'

function ProtectedRoute({ children, roles }) {
  const { isAuthenticated, role } = useAuth()
  if (!isAuthenticated) return <Navigate to="/login-sso" replace />
  if (roles && !roles.includes(role)) return <Navigate to="/" replace />
  return children
}

function AppRoutes() {
  const { isAuthenticated } = useAuth()
  return (
    <Routes>
      <Route path="/login" element={isAuthenticated ? <Navigate to="/" replace /> : <LoginSSO />} />
      <Route path="/login-sso" element={isAuthenticated ? <Navigate to="/" replace /> : <LoginSSO />} />
      <Route path="/callback" element={<Callback />} />

      {[
        { path: '/',                    el: <DashboardPage />,         roles: null },
        { path: '/dashboard-sso',       el: <DashboardSSO />,          roles: null },
        { path: '/interventions',       el: <InterventionsPage />,     roles: null },
        { path: '/interventions/new',   el: <InterventionFormPage />,  roles: [ROLES.ADMIN, ROLES.CHEF] },
        { path: '/interventions/:id',   el: <InterventionDetailPage />,roles: null },
        { path: '/interventions/:id/edit', el: <InterventionFormPage />, roles: [ROLES.ADMIN, ROLES.CHEF] },
        { path: '/users',               el: <UsersPage />,             roles: [ROLES.ADMIN] },
        { path: '/techniciens',         el: <TechniciensPage />,       roles: [ROLES.ADMIN, ROLES.CHEF, ROLES.DIRECTEUR] },
        { path: '/services',            el: <ServicesPage />,          roles: [ROLES.ADMIN, ROLES.DIRECTEUR] },
        { path: '/teams',               el: <ServicesPage />,          roles: [ROLES.ADMIN, ROLES.DIRECTEUR] },
        { path: '/historique',          el: <HistoriquePage />,        roles: [ROLES.ADMIN, ROLES.DIRECTEUR, ROLES.CHEF] },
        { path: '/notifications',       el: <NotificationsPage />,     roles: null },
        { path: '/profile',             el: <ProfilePage />,           roles: null },
      ].map(({ path, el, roles }) => (
        <Route key={path} path={path} element={
          <ProtectedRoute roles={roles}>
            <AppLayout>{el}</AppLayout>
          </ProtectedRoute>
        } />
      ))}

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <SnackbarProvider>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </SnackbarProvider>
      </AuthProvider>
    </ThemeProvider>
  )
}
