import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'gestion-personnel';
const CLIENT_SECRET = 'secret-gestion-personnel-2024';
const REDIRECT_URI = 'http://localhost:5174/auth/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5174';
const SILENT_REDIRECT_URI = 'http://localhost:5174/silent-renew.html';

export const oidcConfig = {
  authority: AUTHORITY,
  client_id: CLIENT_ID,
  client_secret: CLIENT_SECRET,
  redirect_uri: REDIRECT_URI,
  post_logout_redirect_uri: POST_LOGOUT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email roles offline_access gestion-personnel',
  automaticSilentRenew: false, // ❌ Désactivé temporairement (pas de refresh token implementé)
  loadUserInfo: false, // ❌ Désactivé car toutes les infos sont dans id_token
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  
  // ✅ FIX DASHBOARD 403: Mapper les claims custom du JWT
  // oidc-client-ts va lire ces claims depuis l'id_token et les mettre dans user.profile
  // Cela permet d'accéder aux rôles via user.profile.role ou user.profile.roles
  mergeClaimsStrategy: {
    array: ['role', 'permission'], // Ces claims seront traités comme des tableaux
  },
  
  // ✅ Metadata minimale avec SEULEMENT les endpoints implémentés
  metadata: {
    issuer: AUTHORITY,
    authorization_endpoint: `${AUTHORITY}/connect/authorize`,
    token_endpoint: `${AUTHORITY}/connect/token`,
    end_session_endpoint: `${AUTHORITY}/connect/logout`,
  }
};

export const userManager = new UserManager(oidcConfig);

userManager.events.addUserLoaded((user) => {
  console.log('✅ User loaded:', user.profile);
  console.log('✅ User roles:', user.profile.role);
  console.log('✅ User permissions:', user.profile.permission);
});

userManager.events.addUserUnloaded(() => {
  console.log('🚪 User logged out');
});

userManager.events.addAccessTokenExpired(() => {
  console.log('⏱️ Access token expired');
  userManager.signinSilent();
});

userManager.events.addSilentRenewError((error) => {
  console.error('❌ Silent renew error:', error);
});
