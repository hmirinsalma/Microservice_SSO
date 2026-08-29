import React, { useEffect, useState } from 'react';
import { Box, Grid, Typography, Card, CardContent, LinearProgress, Chip } from '@mui/material';
import {
  IconTool, IconEngine, IconCheck, IconX, IconAlertTriangle,
  IconClock, IconTrendingUp, IconBuildingFactory2, IconArrowUpRight
} from '@tabler/icons-react';
import ReactApexChart from 'react-apexcharts';
import { getDashboard } from '../api/dashboardApi';
import { useAuth } from '../contexts/AuthContext';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';
import { formatCurrency } from '../utils/formatters';

interface KpiProps {
  title: string; value: string | number; subtitle?: string;
  icon: React.ReactNode; gradient: string; trend?: number;
}

const KpiCard = ({ title, value, subtitle, icon, gradient, trend }: KpiProps) => (
  <Card sx={{
    cursor: 'default', position: 'relative', overflow: 'hidden',
    '&:hover': { transform: 'translateY(-3px)', boxShadow: '0 12px 30px rgba(0,0,0,0.12)' },
    transition: 'all 0.25s ease',
  }}>
    <Box sx={{ position: 'absolute', top: 0, right: 0, width: 120, height: 120, borderRadius: '0 0 0 100%', background: gradient, opacity: 0.08 }} />
    <CardContent sx={{ p: 2.5 }}>
      <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 2 }}>
        <Box sx={{ width: 44, height: 44, borderRadius: '12px', background: gradient, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff' }}>
          {icon}
        </Box>
        {trend !== undefined && (
          <Chip
            icon={<IconArrowUpRight size={12} />}
            label={`${trend > 0 ? '+' : ''}${trend}%`}
            size="small"
            sx={{ height: 22, fontSize: '0.7rem', fontWeight: 700, bgcolor: trend >= 0 ? '#F0FFF4' : '#FFF5F5', color: trend >= 0 ? '#276749' : '#9B2C2C', border: 'none' }}
          />
        )}
      </Box>
      <Typography sx={{ fontSize: '1.9rem', fontWeight: 800, color: '#1A202C', lineHeight: 1 }}>{value}</Typography>
      <Typography sx={{ fontSize: '0.82rem', fontWeight: 600, color: '#4A5568', mt: 0.5 }}>{title}</Typography>
      {subtitle && <Typography sx={{ fontSize: '0.75rem', color: '#A0AEC0', mt: 0.3 }}>{subtitle}</Typography>}
    </CardContent>
  </Card>
);

const chartOptions = {
  chart: { fontFamily: 'Inter, sans-serif', toolbar: { show: false }, sparkline: { enabled: false } },
  grid: { borderColor: '#F0F4F8', strokeDashArray: 4 },
  legend: { position: 'bottom' as const, fontWeight: 600, fontSize: '13px', markers: { width: 10, height: 10, radius: 5 } },
  tooltip: { theme: 'light', style: { fontFamily: 'Inter, sans-serif' } },
  dataLabels: { enabled: false },
  stroke: { curve: 'smooth' as const, width: 2.5 },
  colors: ['#0066CC', '#00A86B', '#ED8936', '#E53E3E', '#805AD5', '#4299E1'],
};

export default function Dashboard() {
  const { user } = useAuth();
  const [data, setData] = useState<Record<string, unknown> | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getDashboard().then(r => setData(r.data.data as Record<string, unknown>)).finally(() => setLoading(false));
  }, []);

  if (loading) return <Box><PageHeader title="Tableau de bord" /><SkeletonTable rows={6} /></Box>;
  if (!data) return null;

  const role = user?.role;

  if (role === 'Admin_Patrimoine') {
    const d = data as { totalEquipements: number; parEtat: Record<string, number>; maintenancesPlanifiees: number; maintenancesEnRetard: number; coutTotalEstime: number };
    const etatData = Object.entries(d.parEtat || {});
    const disponibles = d.parEtat?.['Disponible'] || 0;
    const tauxDispo = d.totalEquipements > 0 ? Math.round((disponibles / d.totalEquipements) * 100) : 0;

    const pieOptions = {
      ...chartOptions,
      chart: { ...chartOptions.chart, type: 'donut' as const },
      labels: etatData.map(([k]) => k.replace('_', ' ')),
      plotOptions: { pie: { donut: { size: '65%', labels: { show: true, total: { show: true, label: 'Total', fontSize: '14px', fontWeight: '700', color: '#1A202C', formatter: () => `${d.totalEquipements}` } } } } },
    };

    const barOptions = {
      ...chartOptions,
      chart: { ...chartOptions.chart, type: 'bar' as const },
      plotOptions: { bar: { borderRadius: 6, horizontal: false, columnWidth: '55%' } },
      xaxis: { categories: ['Planifiées', 'En retard', 'Terminées'], labels: { style: { fontSize: '12px', fontFamily: 'Inter' } } },
      fill: { gradient: { shade: 'light', type: 'vertical', stops: [0, 100] } },
    };

    return (
      <Box className="fade-in">
        <PageHeader title="Tableau de bord" subtitle={`Bienvenue, ${user?.prenom} — Vue d'ensemble du patrimoine`} crumbs={[{ label: 'Accueil' }]} />
        <Grid container spacing={2.5} sx={{ mb: 3 }}>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Total équipements" value={d.totalEquipements} subtitle="Parc complet" icon={<IconBuildingFactory2 size={22} />} gradient="linear-gradient(135deg,#0066CC,#004999)" trend={2} /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Disponibles" value={disponibles} subtitle={`${tauxDispo}% du parc`} icon={<IconCheck size={22} />} gradient="linear-gradient(135deg,#00A86B,#007A4D)" trend={5} /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Maintenances planifiées" value={d.maintenancesPlanifiees} icon={<IconClock size={22} />} gradient="linear-gradient(135deg,#ED8936,#C05621)" /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="En retard" value={d.maintenancesEnRetard} icon={<IconAlertTriangle size={22} />} gradient="linear-gradient(135deg,#E53E3E,#9B2C2C)" /></Grid>
        </Grid>

        {/* Taux de disponibilité */}
        <Grid container spacing={2.5} sx={{ mb: 2.5 }}>
          <Grid item xs={12} md={4}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Typography variant="h6" sx={{ mb: 2, fontSize: '0.95rem' }}>Taux de disponibilité</Typography>
                <Box sx={{ textAlign: 'center', py: 1 }}>
                  <Typography sx={{ fontSize: '3rem', fontWeight: 800, color: tauxDispo >= 80 ? '#276749' : tauxDispo >= 60 ? '#9C4221' : '#9B2C2C' }}>{tauxDispo}%</Typography>
                  <LinearProgress variant="determinate" value={tauxDispo} sx={{ mt: 2, mb: 1, height: 8, borderRadius: 4, bgcolor: '#F0F4F8', '& .MuiLinearProgress-bar': { background: tauxDispo >= 80 ? 'linear-gradient(90deg,#00A86B,#276749)' : 'linear-gradient(90deg,#ED8936,#C05621)', borderRadius: 4 } }} />
                  <Typography variant="caption" color="text.secondary">{disponibles} sur {d.totalEquipements} équipements disponibles</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={12} md={4}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Typography variant="h6" sx={{ mb: 1, fontSize: '0.95rem' }}>Répartition par état</Typography>
                <ReactApexChart type="donut" options={pieOptions} series={etatData.map(([, v]) => v)} height={220} />
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={12} md={4}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Typography variant="h6" sx={{ mb: 1, fontSize: '0.95rem' }}>Maintenances</Typography>
                <ReactApexChart type="bar" options={barOptions}
                  series={[{ name: 'Nombre', data: [d.maintenancesPlanifiees, d.maintenancesEnRetard, 0] }]} height={220} />
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* Budget */}
        <Grid container spacing={2.5}>
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <Box>
                    <Typography variant="body2" color="text.secondary">Coût total estimé des maintenances</Typography>
                    <Typography sx={{ fontSize: '1.8rem', fontWeight: 800, color: '#0066CC', mt: 0.5 }}>{formatCurrency(d.coutTotalEstime)}</Typography>
                  </Box>
                  <Box sx={{ width: 56, height: 56, borderRadius: '16px', background: 'linear-gradient(135deg,#0066CC,#004999)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff' }}>
                    <IconTrendingUp size={26} />
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Box>
    );
  }

  if (role === 'Directeur') {
    const d = data as { totalEquipements: number; parCategorie: { nom: string; count: number }[]; parService: { nom: string; count: number }[]; equipementsEnIntervention: { id: string; nom: string; reference: string; etat: string; serviceNom: string }[] };
    const barCatOptions = { ...chartOptions, chart: { ...chartOptions.chart, type: 'bar' as const }, plotOptions: { bar: { borderRadius: 6, horizontal: true } }, xaxis: { categories: d.parCategorie?.slice(0, 8).map(c => c.nom) || [] } };
    const barSvcOptions = { ...chartOptions, chart: { ...chartOptions.chart, type: 'bar' as const }, plotOptions: { bar: { borderRadius: 6, columnWidth: '50%' } }, xaxis: { categories: d.parService?.map(s => s.nom) || [] } };

    return (
      <Box className="fade-in">
        <PageHeader title="Tableau de bord" subtitle={`Vue globale du patrimoine — ${user?.prenom} ${user?.nom}`} crumbs={[{ label: 'Accueil' }]} />
        <Grid container spacing={2.5} sx={{ mb: 3 }}>
          <Grid item xs={12} sm={4}><KpiCard title="Total équipements" value={d.totalEquipements} icon={<IconBuildingFactory2 size={22} />} gradient="linear-gradient(135deg,#0066CC,#004999)" /></Grid>
          <Grid item xs={12} sm={4}><KpiCard title="En intervention" value={d.equipementsEnIntervention?.length || 0} icon={<IconAlertTriangle size={22} />} gradient="linear-gradient(135deg,#E53E3E,#9B2C2C)" /></Grid>
          <Grid item xs={12} sm={4}><KpiCard title="Services actifs" value={d.parService?.length || 0} icon={<IconEngine size={22} />} gradient="linear-gradient(135deg,#00A86B,#007A4D)" /></Grid>
        </Grid>
        <Grid container spacing={2.5}>
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Typography variant="h6" sx={{ mb: 1, fontSize: '0.95rem' }}>Équipements par catégorie</Typography>
                <ReactApexChart type="bar" options={barCatOptions} series={[{ name: 'Équipements', data: d.parCategorie?.slice(0, 8).map(c => c.count) || [] }]} height={280} />
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent sx={{ p: 2.5 }}>
                <Typography variant="h6" sx={{ mb: 1, fontSize: '0.95rem' }}>Équipements par service</Typography>
                <ReactApexChart type="bar" options={barSvcOptions} series={[{ name: 'Équipements', data: d.parService?.map(s => s.count) || [] }]} height={280} />
              </CardContent>
            </Card>
          </Grid>
          {d.equipementsEnIntervention?.length > 0 && (
            <Grid item xs={12}>
              <Card>
                <CardContent sx={{ p: 2.5 }}>
                  <Typography variant="h6" sx={{ mb: 2, fontSize: '0.95rem' }}>Équipements nécessitant une intervention</Typography>
                  {d.equipementsEnIntervention.slice(0, 8).map(eq => (
                    <Box key={eq.id} sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', py: 1.2, borderBottom: '1px solid #F0F4F8', '&:last-child': { border: 0 } }}>
                      <Box>
                        <Typography variant="body2" fontWeight={600}>{eq.nom}</Typography>
                        <Typography variant="caption" color="text.secondary">{eq.reference} · {eq.serviceNom}</Typography>
                      </Box>
                      <Chip label={eq.etat.replace('_', ' ')} size="small" sx={{ bgcolor: '#FFF5F5', color: '#9B2C2C', fontWeight: 700, fontSize: '0.72rem' }} />
                    </Box>
                  ))}
                </CardContent>
              </Card>
            </Grid>
          )}
        </Grid>
      </Box>
    );
  }

  if (role === 'Chef_de_Service') {
    const d = data as { totalEquipementsService: number; maintenancesAVenir7j: number; equipementsIndisponibles: number; equipementsRecents30j: number };
    return (
      <Box className="fade-in">
        <PageHeader title="Mon Service" subtitle="Vue des équipements et maintenances de votre service" crumbs={[{ label: 'Accueil' }]} />
        <Grid container spacing={2.5}>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Équipements du service" value={d.totalEquipementsService} icon={<IconBuildingFactory2 size={22} />} gradient="linear-gradient(135deg,#0066CC,#004999)" /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Maintenances à venir (7j)" value={d.maintenancesAVenir7j} icon={<IconClock size={22} />} gradient="linear-gradient(135deg,#ED8936,#C05621)" /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Indisponibles" value={d.equipementsIndisponibles} icon={<IconX size={22} />} gradient="linear-gradient(135deg,#E53E3E,#9B2C2C)" /></Grid>
          <Grid item xs={12} sm={6} md={3}><KpiCard title="Récemment installés (30j)" value={d.equipementsRecents30j} icon={<IconCheck size={22} />} gradient="linear-gradient(135deg,#00A86B,#007A4D)" /></Grid>
        </Grid>
      </Box>
    );
  }

  const d = data as { equipementsAffectes: number; maintenancesAujourdhui: number; prochainesMaintenances7j: number; interventions30j: number };
  return (
    <Box className="fade-in">
      <PageHeader title="Mes missions" subtitle={`Bonjour ${user?.prenom}, voici votre tableau de bord`} crumbs={[{ label: 'Accueil' }]} />
      <Grid container spacing={2.5}>
        <Grid item xs={12} sm={6} md={3}><KpiCard title="Équipements affectés" value={d.equipementsAffectes} icon={<IconTool size={22} />} gradient="linear-gradient(135deg,#0066CC,#004999)" /></Grid>
        <Grid item xs={12} sm={6} md={3}><KpiCard title="Maintenances aujourd'hui" value={d.maintenancesAujourdhui} icon={<IconEngine size={22} />} gradient="linear-gradient(135deg,#ED8936,#C05621)" /></Grid>
        <Grid item xs={12} sm={6} md={3}><KpiCard title="Prochaines (7 jours)" value={d.prochainesMaintenances7j} icon={<IconClock size={22} />} gradient="linear-gradient(135deg,#805AD5,#553C9A)" /></Grid>
        <Grid item xs={12} sm={6} md={3}><KpiCard title="Interventions réalisées (30j)" value={d.interventions30j} icon={<IconCheck size={22} />} gradient="linear-gradient(135deg,#00A86B,#007A4D)" /></Grid>
      </Grid>
    </Box>
  );
}
