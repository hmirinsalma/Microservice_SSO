import React, { useEffect, useState } from 'react';
import { Box, Card, CardContent, Grid, TextField, Button, Typography, Avatar, Divider, CircularProgress, Chip, Alert } from '@mui/material';
import { IconPhone, IconCheck, IconShieldLock, IconBriefcase, IconBuilding, IconInfoCircle, IconExternalLink } from '@tabler/icons-react';
import { useForm } from 'react-hook-form';
import { getProfile, updateProfile } from '../api/usersApi';
import { UserDto } from '../types';
import { useSnackbar } from '../contexts/SnackbarContext';
import PageHeader from '../components/common/PageHeader';
import SkeletonTable from '../components/common/SkeletonTable';

const roleConfig: Record<string, { label: string; bg: string; color: string }> = {
  Admin_Patrimoine: { label: 'Administrateur Patrimoine', bg: '#EBF8FF', color: '#2C5282' },
  Directeur:        { label: 'Directeur',                 bg: '#FAF5FF', color: '#553C9A' },
  Chef_de_Service:  { label: 'Chef de Service',           bg: '#F0FFF4', color: '#276749' },
  Technicien:       { label: 'Technicien Maintenance',    bg: '#FFFAF0', color: '#9C4221' },
};

const avatarGradients: Record<string, string> = {
  Admin_Patrimoine: 'linear-gradient(135deg,#0066CC,#004999)',
  Directeur:        'linear-gradient(135deg,#7B2FBE,#5A1F8C)',
  Chef_de_Service:  'linear-gradient(135deg,#00A86B,#007A4D)',
  Technicien:       'linear-gradient(135deg,#ED8936,#C05621)',
};

export default function MonProfil() {
  const { showSuccess, showError } = useSnackbar();
  const [profile, setProfile] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset } = useForm<{ telephone: string }>();

  useEffect(() => {
    getProfile()
      .then(r => {
        setProfile(r.data.data);
        reset({ telephone: r.data.data.telephone });
      })
      .finally(() => setLoading(false));
  }, []);

  const onSave = async (d: { telephone: string }) => {
    setSaving(true);
    try {
      const updated = await updateProfile({ telephone: d.telephone });
      setProfile(updated.data.data);
      showSuccess('Téléphone mis à jour.');
    } catch { showError('Erreur lors de la mise à jour.'); }
    finally { setSaving(false); }
  };

  if (loading) return <Box sx={{ p: 3 }}><SkeletonTable rows={5} /></Box>;

  const cfg  = roleConfig[profile?.role || ''] || { label: profile?.role, bg: '#F7FAFC', color: '#4A5568' };
  const grad = avatarGradients[profile?.role || ''] || 'linear-gradient(135deg,#0066CC,#004999)';
  const initials = profile ? `${profile.prenom[0]}${profile.nom[0]}`.toUpperCase() : 'U';

  return (
    <Box className="fade-in">
      <PageHeader
        title="Mon Profil"
        subtitle="Informations de votre compte EAMS"
        crumbs={[{ label: 'Accueil', to: '/' }, { label: 'Mon Profil' }]}
      />

      <Grid container spacing={3}>
        {/* Carte identité */}
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent sx={{ textAlign: 'center', py: 4, px: 3 }}>
              <Avatar sx={{
                width: 88, height: 88, fontSize: '1.8rem', fontWeight: 800,
                mx: 'auto', mb: 2, background: grad,
                boxShadow: '0 8px 24px rgba(0,0,0,0.18)',
              }}>
                {initials}
              </Avatar>
              <Typography variant="h5" fontWeight={700}>{profile?.prenom} {profile?.nom}</Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5, mb: 2 }}>{profile?.email}</Typography>
              <Chip label={cfg.label} sx={{ bgcolor: cfg.bg, color: cfg.color, fontWeight: 700, border: 'none', mb: 3 }} />

              <Divider sx={{ mb: 2.5 }} />

              {[
                { icon: <IconBriefcase size={16} />, label: 'Poste',    value: profile?.poste },
                { icon: <IconBuilding  size={16} />, label: 'Service',  value: profile?.serviceNom || 'Direction générale' },
                { icon: <IconPhone     size={16} />, label: 'Téléphone',value: profile?.telephone || '—' },
                { icon: <IconShieldLock size={16} />,label: 'Rôle',     value: cfg.label },
              ].map(row => (
                <Box key={row.label} sx={{
                  display: 'flex', alignItems: 'center', gap: 1.5, py: 1,
                  borderBottom: '1px solid #F0F4F8', '&:last-child': { border: 0 },
                }}>
                  <Box sx={{ color: '#A0AEC0', flexShrink: 0 }}>{row.icon}</Box>
                  <Box sx={{ flex: 1, textAlign: 'left' }}>
                    <Typography sx={{ fontSize: '0.72rem', color: '#A0AEC0', lineHeight: 1 }}>{row.label}</Typography>
                    <Typography sx={{ fontSize: '0.875rem', fontWeight: 600, mt: 0.2 }}>{row.value}</Typography>
                  </Box>
                </Box>
              ))}
            </CardContent>
          </Card>
        </Grid>

        {/* Panneau droite */}
        <Grid item xs={12} md={8}>
          {/* Modifier le téléphone */}
          <Card sx={{ mb: 3 }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2.5 }}>
                <Box sx={{ width: 36, height: 36, borderRadius: '10px', bgcolor: '#EBF8FF', color: '#2C5282', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <IconPhone size={18} />
                </Box>
                <Box>
                  <Typography variant="h6" sx={{ fontSize: '1rem', fontWeight: 700 }}>Modifier le téléphone</Typography>
                  <Typography variant="body2" color="text.secondary">Mettez à jour votre numéro de contact</Typography>
                </Box>
              </Box>
              <form onSubmit={handleSubmit(onSave)}>
                <Grid container spacing={2} alignItems="flex-end">
                  <Grid item xs={12} sm={8}>
                    <TextField
                      {...register('telephone')}
                      label="Numéro de téléphone" fullWidth size="small"
                      placeholder="+212 6XXXXXXXX"
                    />
                  </Grid>
                  <Grid item xs={12} sm={4}>
                    <Button type="submit" variant="contained" fullWidth disabled={saving}
                      startIcon={saving ? null : <IconCheck size={16} />}
                      sx={{ background: 'linear-gradient(135deg,#0066CC,#004999)' }}>
                      {saving ? <CircularProgress size={18} color="inherit" /> : 'Enregistrer'}
                    </Button>
                  </Grid>
                </Grid>
              </form>
            </CardContent>
          </Card>

          {/* Notice SSO — mot de passe */}
          <Card sx={{ border: '1px solid #BEE3F8' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.5 }}>
                <Box sx={{ width: 36, height: 36, borderRadius: '10px', bgcolor: '#EBF8FF', color: '#2C5282', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0, mt: 0.2 }}>
                  <IconShieldLock size={18} />
                </Box>
                <Box>
                  <Typography variant="h6" sx={{ fontSize: '1rem', fontWeight: 700, mb: 0.5 }}>
                    Sécurité du compte
                  </Typography>
                  <Alert
                    severity="info"
                    icon={<IconInfoCircle size={18} />}
                    sx={{ borderRadius: '10px', bgcolor: '#EBF8FF', border: '1px solid #BEE3F8', color: '#2C5282', fontSize: '0.875rem', mb: 1.5 }}
                  >
                    La gestion du mot de passe et de l'authentification sera assurée par le microservice SSO de l'ONEE.
                  </Alert>
                  <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.7 }}>
                    Pour modifier votre mot de passe ou gérer la sécurité de votre compte, connectez-vous au portail d'identité SSO de l'ONEE.
                    Cette fonctionnalité sera accessible directement depuis cette application après l'intégration du microservice SSO.
                  </Typography>
                  <Box sx={{ mt: 2 }}>
                    <Button
                      variant="outlined" size="small"
                      startIcon={<IconExternalLink size={15} />}
                      disabled
                      sx={{ borderColor: '#BEE3F8', color: '#2C5282', fontSize: '0.8rem' }}
                    >
                      Portail SSO (disponible après intégration)
                    </Button>
                  </Box>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
