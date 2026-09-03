import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

/**
 * Page Callback SSO pour EAMS
 * Gère le retour depuis ONEE.SSO après authentification
 */
export default function Callback() {
  const navigate = useNavigate();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        console.log('🔐 [EAMS] Starting SSO callback...');
        const params = new URLSearchParams(window.location.search);
        const code = params.get('code');
        const state = params.get('state');

        console.log('📋 [EAMS] Callback params:', { code, state });

        if (!code) {
          console.error('❌ [EAMS] Missing authorization code');
          alert('Erreur : Code d\'autorisation manquant');
          navigate('/login');
          return;
        }

        // 1. Échanger le code contre un token SSO
        console.log('🔄 [EAMS] Exchanging code for token with SSO...');
        const ssoTokenResponse = await fetch('http://localhost:5205/connect/token', {
          method: 'POST',
          headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
          body: new URLSearchParams({
            grant_type: 'authorization_code',
            code,
            redirect_uri: 'http://localhost:5173/auth/callback',
            client_id: 'eams-spa',
            client_secret: 'secret_eams_spa_2024',
          }),
        });

        if (!ssoTokenResponse.ok) {
          throw new Error(`SSO token exchange failed: ${ssoTokenResponse.status}`);
        }

        const { access_token } = await ssoTokenResponse.json();
        console.log('✅ [EAMS] SSO access token received');

        // 2. Appeler le callback EAMS avec le token SSO pour déclencher l'auto-provisioning
        console.log('🔄 [EAMS] Calling EAMS callback for auto-provisioning...');
        const response = await fetch('http://localhost:5137/api/auth/sso-callback', {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${access_token}`,
            'Content-Type': 'application/json',
          },
        });

        console.log('📡 [EAMS] Response status:', response.status);

        if (response.status === 401) {
          // Accès refusé (pas de rôle @eams-spa)
          console.warn('⚠️ [EAMS] Access denied (401)');
          alert('ACCÈS REFUSÉ À EAMS\\n\\nVous n\'avez pas les permissions nécessaires pour accéder à cette application.');
          window.location.href = 'http://localhost:5205';
          return;
        }

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}`);
        }

        const { data } = await response.json();
        console.log('✅ [EAMS] Auto-provisioning successful');

        const eamsToken = data.token;
        if (!eamsToken) {
          throw new Error('Token EAMS manquant dans la réponse');
        }

        // 3. Utiliser directement les données du callback (pas besoin d'appeler /api/profile)
        const authUser = {
          id: data.userId,
          nom: data.nom,
          prenom: data.prenom,
          email: data.email,
          role: data.role as any, // ✅ Déjà nettoyé par le backend
          serviceId: data.serviceId,
          token: eamsToken,
          expiresAt: data.expiresAt,
        };

        // 4. Stocker dans localStorage
        localStorage.setItem('token', eamsToken);
        localStorage.setItem('user', JSON.stringify(authUser));

        console.log('✅ [EAMS] User stored in localStorage:', authUser);

        // 5. Rediriger vers le dashboard
        window.location.href = '/'; // Dashboard est à la racine
      } catch (error) {
        console.error('❌ [EAMS] Erreur lors du callback:', error);
        alert('Erreur lors de la connexion SSO. Veuillez réessayer.');
        navigate('/login');
      }
    };

    completeLogin();
  }, [navigate]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="text-center">
        <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        <p className="mt-4 text-gray-600 font-medium">Authentification en cours...</p>
        <p className="text-sm text-gray-500 mt-2">Veuillez patienter</p>
      </div>
    </div>
  );
}
