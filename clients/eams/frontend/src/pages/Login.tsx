import React, { useState } from 'react';
import { Box, Typography, TextField, Button, Alert, CircularProgress, InputAdornment, IconButton } from '@mui/material';
import { IconEye, IconEyeOff, IconLock, IconMail, IconBuildingFactory2 } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('admin@onee.ma');
  const [password, setPassword] = useState('Admin@1234');
  const [showPwd, setShowPwd] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(email, password);
      navigate('/');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: { message?: string } } } })?.response?.data?.error?.message;
      setError(msg || 'Identifiant ou mot de passe incorrect.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex' }}>
      {/* Left panel */}
      <Box sx={{
        display: { xs: 'none', lg: 'flex' }, width: '55%', flexDirection: 'column',
        background: 'linear-gradient(135deg, #0A1628 0%, #0F2040 50%, #0066CC 100%)',
        p: 6, position: 'relative', overflow: 'hidden', justifyContent: 'space-between',
      }}>
        {/* Background decoration */}
        <Box sx={{ position: 'absolute', top: -100, right: -100, width: 400, height: 400, borderRadius: '50%', background: 'rgba(0,102,204,0.15)', blur: '80px' }} />
        <Box sx={{ position: 'absolute', bottom: -50, left: -50, width: 300, height: 300, borderRadius: '50%', background: 'rgba(0,168,107,0.1)' }} />

        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 6 }}>
            <Box sx={{ width: 48, height: 48, borderRadius: '14px', background: 'linear-gradient(135deg,#0066CC,#00A86B)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <IconBuildingFactory2 size={26} color="white" />
            </Box>
            <Box>
              <Typography sx={{ color: '#fff', fontWeight: 800, fontSize: '1.3rem', lineHeight: 1 }}>ONEE EAMS</Typography>
              <Typography sx={{ color: 'rgba(255,255,255,0.5)', fontSize: '0.75rem' }}>Enterprise Asset Management</Typography>
            </Box>
          </Box>

          <Typography sx={{ color: '#fff', fontWeight: 800, fontSize: '2.5rem', lineHeight: 1.2, mb: 2 }}>
            Gérez votre patrimoine technique avec confiance
          </Typography>
          <Typography sx={{ color: 'rgba(255,255,255,0.6)', fontSize: '1rem', lineHeight: 1.7, maxWidth: 440 }}>
            Plateforme de gestion des équipements, maintenances et actifs techniques de l'ONEE.
            Sécurisée, performante et conçue pour les équipes de terrain.
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', gap: 4 }}>
          {[['300+', 'Équipements'], ['800+', 'Maintenances'], ['4', 'Rôles']].map(([v, l]) => (
            <Box key={l}>
              <Typography sx={{ color: '#60A5FA', fontWeight: 800, fontSize: '1.8rem', lineHeight: 1 }}>{v}</Typography>
              <Typography sx={{ color: 'rgba(255,255,255,0.5)', fontSize: '0.8rem', mt: 0.5 }}>{l}</Typography>
            </Box>
          ))}
        </Box>
      </Box>

      {/* Right panel */}
      <Box sx={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', p: 4, bgcolor: '#F0F4F8' }}>
        <Box sx={{ width: '100%', maxWidth: 420 }}>
          <Box sx={{ mb: 4, display: { lg: 'none' }, textAlign: 'center' }}>
            <Box sx={{ width: 56, height: 56, borderRadius: '16px', background: 'linear-gradient(135deg,#0066CC,#00A86B)', display: 'flex', alignItems: 'center', justifyContent: 'center', mx: 'auto', mb: 1.5 }}>
              <IconBuildingFactory2 size={28} color="white" />
            </Box>
            <Typography variant="h5" fontWeight={800}>ONEE EAMS</Typography>
          </Box>

          <Box sx={{ bgcolor: '#fff', borderRadius: '20px', p: 4, boxShadow: '0 4px 24px rgba(0,0,0,0.08)', border: '1px solid #E2E8F0' }}>
            <Typography variant="h5" fontWeight={700} sx={{ mb: 0.5 }}>Connexion</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3.5 }}>Entrez vos identifiants pour accéder à votre espace</Typography>

            <form onSubmit={handleSubmit}>
              <Box sx={{ mb: 2.5 }}>
                <Typography sx={{ fontSize: '0.82rem', fontWeight: 600, mb: 0.8, color: '#4A5568' }}>Adresse email</Typography>
                <TextField
                  fullWidth size="small" type="email" value={email}
                  onChange={(e) => setEmail(e.target.value)} required autoFocus
                  placeholder="votre@email.com"
                  InputProps={{ startAdornment: <InputAdornment position="start"><IconMail size={16} color="#A0AEC0" /></InputAdornment> }}
                />
              </Box>

              <Box sx={{ mb: 3 }}>
                <Typography sx={{ fontSize: '0.82rem', fontWeight: 600, mb: 0.8, color: '#4A5568' }}>Mot de passe</Typography>
                <TextField
                  fullWidth size="small" type={showPwd ? 'text' : 'password'} value={password}
                  onChange={(e) => setPassword(e.target.value)} required
                  placeholder="••••••••"
                  InputProps={{
                    startAdornment: <InputAdornment position="start"><IconLock size={16} color="#A0AEC0" /></InputAdornment>,
                    endAdornment: <InputAdornment position="end">
                      <IconButton size="small" onClick={() => setShowPwd(!showPwd)} edge="end" sx={{ color: '#A0AEC0' }}>
                        {showPwd ? <IconEyeOff size={16} /> : <IconEye size={16} />}
                      </IconButton>
                    </InputAdornment>
                  }}
                />
              </Box>

              {error && <Alert severity="error" sx={{ mb: 2.5, borderRadius: '10px', fontSize: '0.82rem' }}>{error}</Alert>}

              <Button type="submit" variant="contained" fullWidth size="large" disabled={loading}
                sx={{ py: 1.4, borderRadius: '12px', fontSize: '0.95rem', background: 'linear-gradient(135deg,#0066CC,#004999)', boxShadow: '0 4px 15px rgba(0,102,204,0.35)' }}>
                {loading ? <CircularProgress size={22} color="inherit" /> : 'Se connecter'}
              </Button>
            </form>
          </Box>

          <Typography align="center" sx={{ mt: 3, fontSize: '0.78rem', color: '#A0AEC0' }}>
            © 2026 ONEE — Office National de l'Électricité et de l'Eau Potable
          </Typography>
        </Box>
      </Box>
    </Box>
  );
}
