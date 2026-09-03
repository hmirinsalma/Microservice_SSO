import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../../auth/authService';

const Callback = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        const user = await authService.completeLogin();
        
        // 🎯 AUTO-PROVISIONING: Appeler le backend pour créer l'utilisateur si nécessaire
        console.log('📡 [TIMS] Calling /api/auth/sso-callback for auto-provisioning...');
        
        let timsToken = user.access_token; // Par défaut, utiliser le token SSO
        let accessGranted = false; // ✅ Flag pour contrôler l'accès
        
        try {
          const response = await fetch('http://localhost:5178/api/auth/sso-callback', {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${user.access_token}`,
              'Content-Type': 'application/json'
            }
          });

          if (response.ok) {
            const provisioningResult = await response.json();
            console.log('✅ [TIMS] Auto-provisioning successful:', provisioningResult);
            
            // ✅ IMPORTANT: Utiliser le token TIMS retourné par le callback
            if (provisioningResult.data && provisioningResult.data.token) {
              timsToken = provisioningResult.data.token;
              console.log('🔑 [TIMS] Using TIMS local token instead of SSO token');
            }
            
            accessGranted = true; // ✅ Accès autorisé
          } else {
            // ❌ ACCÈS REFUSÉ: Afficher un message d'erreur clair
            console.error('❌ [TIMS] Accès refusé:', response.status);
            
            const errorData = await response.json().catch(() => ({ message: 'Accès refusé' }));
            
            alert(`🚫 ACCÈS REFUSÉ À TIMS\n\n${errorData.message || 'Vous ne possédez pas les autorisations nécessaires pour accéder à cette application.'}\n\nVeuillez contacter votre administrateur.`);
            
            // Rediriger vers la page de login du SSO
            localStorage.clear();
            window.location.href = 'http://localhost:5205';
            return; // ✅ IMPORTANT: Arrêter l'exécution ici
          }
        } catch (provisioningError) {
          console.error('⚠️ [TIMS] Auto-provisioning error:', provisioningError);
          alert('🚫 ERREUR DE CONNEXION\n\nImpossible de vérifier vos autorisations.\n\nVeuillez réessayer.');
          localStorage.clear();
          window.location.href = 'http://localhost:5205';
          return; // ✅ Arrêter en cas d'erreur réseau
        }
        
        // ✅ Si on arrive ici, l'accès est autorisé
        if (!accessGranted) {
          return; // Double sécurité
        }
        
        // Récupérer les custom claims TIMS
        const timsContext = await authService.getTimsContext();
        console.log('🎯 TIMS Context:', timsContext);
        
        // 🔥 AMÉLIORATION: Appeler d'abord le provisioning SSO, puis récupérer le profil
        let authUser;
        try {
          // 🎯 ÉTAPE 1: Appeler le callback SSO pour déclencher l'auto-provisioning
          const callbackResponse = await fetch('http://localhost:5178/api/auth/sso-callback', {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${timsToken}`, // ✅ Utiliser le token TIMS
            }
          });
          
          if (callbackResponse.ok) {
            const callbackData = await callbackResponse.json();
            const provisioned = callbackData.data;
            console.log('✅ [TIMS] Auto-provisioning réussi:', provisioned);
            
            authUser = {
              id: provisioned.user.id,
              firstName: provisioned.user.firstName,
              lastName: provisioned.user.lastName,
              email: provisioned.user.email,
              roles: provisioned.user.roles, // ✅ Array from backend
              serviceId: provisioned.user.serviceId,
              equipeId: provisioned.user.equipeId,
              token: provisioned.token, // ✅ Nouveau token généré par le backend
              expiresAt: provisioned.expiresAt,
            };
            
            // ✅ Utiliser le nouveau token généré par le backend
            timsToken = provisioned.token;
          } else {
            throw new Error('Callback SSO failed');
          }
        } catch (profileError) {
          console.warn('⚠️ [TIMS] Could not fetch profile, using claims:', profileError);
          // Fallback vers les claims si le backend est inaccessible
          // Gérer les arrays pour les multi-rôles
          const firstNameRaw = user.profile.given_name;
          const lastNameRaw = user.profile.family_name;
          const roleRaw = user.profile.role;
          
          // ✅ FIX: Chercher UNIQUEMENT le rôle TIMS (avec @tims-app)
          const roleFromClaims = Array.isArray(roleRaw) 
            ? roleRaw.find(r => r.includes('@tims-app')) || roleRaw[0] 
            : (roleRaw || 'Technicien');
          const cleanRole = roleFromClaims.includes('@') 
            ? roleFromClaims.split('@')[0] 
            : roleFromClaims;
          
          authUser = {
            id: user.profile.sub,
            firstName: Array.isArray(firstNameRaw) ? firstNameRaw[0] : (firstNameRaw || 'À'),
            lastName: Array.isArray(lastNameRaw) ? lastNameRaw[0] : (lastNameRaw || 'renseigner'),
            email: user.profile.email,
            roles: [cleanRole], // ✅ Array pour compatibilité avec AuthContext
            serviceId: user.profile.serviceId || null,
            equipeId: user.profile.equipeId || null,
            token: timsToken, // ✅ Token TIMS
            expiresAt: user.expires_at || Date.now() + 3600000,
          };
        }
        
        localStorage.setItem('tims_token', timsToken); // ✅ Stocker le token TIMS
        localStorage.setItem('tims_user', JSON.stringify(authUser));
        console.log('✅ [TIMS] User stored in localStorage:', authUser);
        
        // Forcer le rechargement du contexte en naviguant
        window.location.href = '/dashboard';
      } catch (error) {
        console.error('❌ Erreur lors du callback:', error);
        navigate('/login');
      }
    };

    completeLogin();
  }, [navigate]);

  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h2>Authentification TIMS en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
