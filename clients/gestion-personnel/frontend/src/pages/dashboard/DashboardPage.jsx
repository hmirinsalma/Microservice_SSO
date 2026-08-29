import { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import dashboardApi from '../../api/dashboardApi';
import Spinner from '../../components/ui/Spinner';
import AdminDashboard from './AdminDashboard';
import DirecteurDashboard from './DirecteurDashboard';
import ChefServiceDashboard from './ChefServiceDashboard';
import EmployeDashboard from './EmployeDashboard';

export default function DashboardPage() {
  const { user } = useAuth();
  const [data, setData]       = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState(false);

  useEffect(() => {
    dashboardApi.get()
      .then(({ data: r }) => setData(r))
      .catch(() => setError(true))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Spinner message="Chargement du tableau de bord..." />;
  if (error)   return <div className="text-center py-20 text-red-500 text-sm">Erreur de chargement. Vérifiez que votre fiche employé est liée à votre compte.</div>;

  const role = user?.role;
  if (role === 'AdministrateurRH') return <AdminDashboard data={data} />;
  if (role === 'Directeur')        return <DirecteurDashboard data={data} />;
  if (role === 'ChefDeService')    return <ChefServiceDashboard data={data} />;
  if (role === 'Employe')          return <EmployeDashboard data={data} />;

  return <div className="text-slate-500 text-sm">Rôle non reconnu.</div>;
}
