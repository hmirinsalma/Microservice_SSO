import React, { useEffect, useState } from 'react';
import { Box, Card, CardContent, Grid, Typography, Button, Divider, Avatar, Chip } from '@mui/material';
import { IconEdit, IconArrowLeft, IconMapPin, IconCalendar, IconTag, IconBuildingFactory2, IconUser, IconShield, IconCurrencyDollar, IconTruck } from '@tabler/icons-react';
import { useParams, useNavigate } from 'react-router-dom';
import { getEquipement, getHistorique } from '../api/equipementsApi';
import { EquipementDetailDto, HistoriqueEntryDto } from '../types';
import { useAuth } from '../contexts/AuthContext';
import { canCreate } from '../utils/roleGuard';
import { formatDate, formatCurrency } from '../utils/formatters';
import StatusBadge from '../components/common/StatusBadge';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';

const histoIcons: Record<string, string> = {
  Creation: '🆕', ChangementEtat: '🔄', ChangementResponsable: '👤', ChangementLocalisation: '📍',
  Modification: '✏️', Maintenance: '🔧'
};

export default function EquipementDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [eq, setEq] = useState<EquipementDetailDto | null>(null);
  const [historique, setHistorique] = useState<HistoriqueEntryDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;
    Promise.all([getEquipement(id), getHistorique(id, 1, 15)])
      .then(([eqRes, histRes]) => {
        setEq(eqRes.data.data);
        setHistorique(histRes.data.data.items);
      }).finally(() => setLoading(false));
  }, [id]);

  if (loading) return <Box sx={{ p: 3 }}><SkeletonTable rows={8} /></Box>;
  if (!eq) return null;

  const infoFields = [
    { icon: <IconTag size={15} />, label: 'Type', value: eq.type },
    { icon: <IconBuildingFactory2 size={15} />, label: 'Marque', value: eq.marque },
    { icon: <IconBuildingFactory2 size={15} />, label: 'Modèle', value: eq.modele },
    { icon: <IconTag size={15} />, label: 'N° de série', value: eq.numeroSerie, mono: true },
    { icon: <IconMapPin size={15} />, label: 'Localisation', value: eq.localisation },
    { icon: <IconBuildingFactory2 size={15} />, label: 'Service', value: eq.serviceNom },
    { icon: <IconUser size={15} />, label: 'Responsable', value: eq.responsableNom },
    { icon: <IconCalendar size={15} />, label: 'Date d\'installation', value: formatDate(eq.dateInstallation) },
    { icon: <IconCalendar size={15} />, label: 'Mise en service', value: formatDate(eq.dateMiseEnService) },
    { icon: <IconShield size={15} />, label: 'Fin de garantie', value: formatDate(eq.dateFinGarantie) },
    { icon: <IconCurrencyDollar size={15} />, label: 'Valeur d\'acquisition', value: formatCurrency(eq.valeurAcquisition) },
    { icon: <IconTruck size={15} />, label: 'Fournisseur', value: eq.fournisseur || '—' },
  ];

  return (
    <Box className="fade-in">
      <PageHeader
        title={eq.nom}
        subtitle={eq.reference}
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Équipements', to: '/equipements' }, { label: eq.nom }]}
        action={
          <Box sx={{ display: 'flex', gap: 1.5 }}>
            <Button startIcon={<IconArrowLeft size={16} />} onClick={() => navigate('/equipements')}
              variant="outlined" sx={{ borderColor: '#E2E8F0', color: '#718096' }}>
              Retour
            </Button>
            {canCreate(user!.role) && (
              <Button variant="contained" startIcon={<IconEdit size={16} />}
                onClick={() => navigate(`/equipements/${id}/modifier`)}
                sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)' }}>
                Modifier
              </Button>
            )}
          </Box>
        }
      />

      <Grid container spacing={3}>
        {/* Main info */}
        <Grid item xs={12} lg={8}>
          <Card sx={{ mb: 3 }}>
            <CardContent sx={{ p: 3 }}>
              {/* Header */}
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
                <Box sx={{
                  width: 56, height: 56, borderRadius: '14px', flexShrink: 0,
                  background: `${eq.couleurCategorie}20`, border: `2px solid ${eq.couleurCategorie}40`,
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  <Typography sx={{ fontSize: '0.8rem', fontWeight: 800, color: eq.couleurCategorie }}>
                    {eq.categorieCode}
                  </Typography>
                </Box>
                <Box sx={{ flex: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, flexWrap: 'wrap' }}>
                    <StatusBadge etat={eq.etat} size="medium" />
                    <Chip label={eq.categorieName} size="small"
                      sx={{ bgcolor: `${eq.couleurCategorie}15`, color: eq.couleurCategorie, border: `1px solid ${eq.couleurCategorie}30`, fontWeight: 700 }} />
                    <Typography sx={{ fontFamily: 'monospace', fontSize: '0.8rem', color: '#0066CC', bgcolor: '#EBF8FF', px: 1, py: 0.3, borderRadius: '6px', fontWeight: 700 }}>
                      {eq.reference}
                    </Typography>
                  </Box>
                </Box>
              </Box>

              <Divider sx={{ mb: 2.5 }} />

              {/* Info grid */}
              <Grid container spacing={2}>
                {infoFields.map(f => (
                  <Grid item xs={12} sm={6} key={f.label}>
                    <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start' }}>
                      <Box sx={{ color: '#A0AEC0', mt: 0.3, flexShrink: 0 }}>{f.icon}</Box>
                      <Box>
                        <Typography sx={{ fontSize: '0.72rem', color: '#A0AEC0', textTransform: 'uppercase', letterSpacing: '0.05em', fontWeight: 600 }}>{f.label}</Typography>
                        <Typography sx={{ fontSize: '0.875rem', fontWeight: 600, mt: 0.2, fontFamily: (f as { mono?: boolean }).mono ? 'monospace' : 'inherit' }}>{f.value}</Typography>
                      </Box>
                    </Box>
                  </Grid>
                ))}
                {eq.description && (
                  <Grid item xs={12}>
                    <Divider sx={{ my: 1 }} />
                    <Typography sx={{ fontSize: '0.72rem', color: '#A0AEC0', textTransform: 'uppercase', letterSpacing: '0.05em', fontWeight: 600, mb: 0.5 }}>Description</Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.7 }}>{eq.description}</Typography>
                  </Grid>
                )}
              </Grid>
            </CardContent>
          </Card>

          {/* Photos */}
          {eq.photos.length > 0 && (
            <Card>
              <CardContent sx={{ p: 3 }}>
                <Typography variant="h6" sx={{ fontSize: '1rem', fontWeight: 700, mb: 2 }}>Photos ({eq.photos.length})</Typography>
                <Box sx={{ display: 'flex', gap: 1.5, flexWrap: 'wrap' }}>
                  {eq.photos.map(p => (
                    <Box key={p.id} component="img" src={p.url} alt=""
                      sx={{ width: 120, height: 90, objectFit: 'cover', borderRadius: '10px', border: p.isMain ? '2px solid #0066CC' : '2px solid #E2E8F0', cursor: 'pointer', '&:hover': { transform: 'scale(1.03)' }, transition: 'transform 0.2s' }} />
                  ))}
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>

        {/* Timeline historique */}
        <Grid item xs={12} lg={4}>
          <Card sx={{ position: 'sticky', top: 80 }}>
            <CardContent sx={{ p: 3 }}>
              <Typography variant="h6" sx={{ fontSize: '1rem', fontWeight: 700, mb: 2.5 }}>
                Historique
              </Typography>
              {historique.length === 0 ? (
                <Typography variant="body2" color="text.secondary">Aucun historique enregistré.</Typography>
              ) : (
                <Box sx={{ position: 'relative' }}>
                  {/* Ligne verticale */}
                  <Box sx={{ position: 'absolute', left: 13, top: 6, bottom: 6, width: 2, bgcolor: '#E2E8F0', borderRadius: 1 }} />

                  {historique.map((h, i) => (
                    <Box key={h.id} sx={{ display: 'flex', gap: 2, mb: 2.5, position: 'relative' }}>
                      {/* Dot */}
                      <Box sx={{
                        width: 28, height: 28, borderRadius: '50%', flexShrink: 0, zIndex: 1,
                        bgcolor: '#fff', border: '2px solid #E2E8F0',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        fontSize: '0.85rem',
                      }}>
                        {histoIcons[h.typeEvenement] || '📋'}
                      </Box>
                      <Box sx={{ flex: 1, pt: 0.3 }}>
                        <Typography sx={{ fontSize: '0.82rem', fontWeight: 600, lineHeight: 1.3, color: '#1A202C' }}>
                          {h.typeEvenement.replace(/([A-Z])/g, ' $1').trim()}
                        </Typography>
                        {(h.valeurAvant || h.valeurApres) && (
                          <Box sx={{ mt: 0.5, p: 1, borderRadius: '6px', bgcolor: '#F7FAFC', fontSize: '0.72rem', fontFamily: 'monospace' }}>
                            {h.valeurAvant && <Box sx={{ color: '#9B2C2C' }}>- {h.valeurAvant}</Box>}
                            {h.valeurApres && <Box sx={{ color: '#276749' }}>+ {h.valeurApres}</Box>}
                          </Box>
                        )}
                        <Typography sx={{ fontSize: '0.72rem', color: '#A0AEC0', mt: 0.5 }}>
                          {h.auteurNom} · {new Date(h.horodatageUtc).toLocaleString('fr-FR')}
                        </Typography>
                      </Box>
                    </Box>
                  ))}
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
