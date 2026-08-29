import React, { useEffect, useState } from 'react';
import { Box, Card, CardContent, Typography, Grid, CircularProgress } from '@mui/material';
import authService from '../../auth/authService';
import apiClient from '../../api/axiosConfig';

const DashboardSSO = () => {
  const [user, setUser] = useState(null);
  const [timsContext, setTimsContext] = useState(null);
  const [apiTestResult, setApiTestResult] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadUserData = async () => {
      try {
        // Récupérer le profil utilisateur
        const profile = await authService.getUserProfile();
        setUser(profile);

        // Récupérer le contexte TIMS (custom claims)
        const context = await authService.getTimsContext();
        setTimsContext(context);

        // Tester l'appel API avec les custom headers
        const response = await apiClient.get('/testsso/verify-claims');
        setApiTestResult(response.data.data);

        setLoading(false);
      } catch (error) {
        console.error('❌ Erreur lors du chargement des données:', error);
        setLoading(false);
      }
    };

    loadUserData();
  }, []);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        🎯 Dashboard SSO TIMS
      </Typography>
      <Typography variant="subtitle1" color="text.secondary" gutterBottom>
        Test d'intégration SSO avec custom claims
      </Typography>

      <Grid container spacing={3} sx={{ mt: 2 }}>
        {/* User Profile */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                👤 Profil Utilisateur
              </Typography>
              {user && (
                <Box>
                  <Typography><strong>Nom:</strong> {user.name || 'N/A'}</Typography>
                  <Typography><strong>Email:</strong> {user.email || 'N/A'}</Typography>
                  <Typography><strong>Sub:</strong> {user.sub || 'N/A'}</Typography>
                  <Typography><strong>Rôles:</strong> {user.roles?.join(', ') || 'N/A'}</Typography>
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* TIMS Custom Claims */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                ⭐ Custom Claims TIMS
              </Typography>
              {timsContext && (
                <Box>
                  <Typography><strong>TIMS User ID:</strong> {timsContext.userId || 'N/A'}</Typography>
                  <Typography><strong>TIMS Service ID:</strong> {timsContext.serviceId || 'N/A'}</Typography>
                  <Typography><strong>TIMS Team ID:</strong> {timsContext.teamId || 'N/A'}</Typography>
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* API Test Result */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                ✅ Résultat du test API Backend
              </Typography>
              {apiTestResult && (
                <Box>
                  <Typography variant="body1" color="success.main" gutterBottom>
                    {apiTestResult.message}
                  </Typography>
                  
                  <Typography variant="subtitle2" sx={{ mt: 2, mb: 1 }}>
                    Custom Claims reçus par le backend:
                  </Typography>
                  <Box sx={{ pl: 2 }}>
                    <Typography>• tims_user_id: {apiTestResult.customClaims?.tims_user_id || 'N/A'}</Typography>
                    <Typography>• tims_service_id: {apiTestResult.customClaims?.tims_service_id || 'N/A'}</Typography>
                    <Typography>• tims_team_id: {apiTestResult.customClaims?.tims_team_id || 'N/A'}</Typography>
                  </Box>

                  <Typography variant="subtitle2" sx={{ mt: 2, mb: 1 }}>
                    Standard Claims:
                  </Typography>
                  <Box sx={{ pl: 2 }}>
                    <Typography>• Email: {apiTestResult.standardClaims?.email || 'N/A'}</Typography>
                    <Typography>• Sub: {apiTestResult.standardClaims?.sub || 'N/A'}</Typography>
                    <Typography>• Rôles: {apiTestResult.standardClaims?.roles?.join(', ') || 'N/A'}</Typography>
                  </Box>
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default DashboardSSO;
