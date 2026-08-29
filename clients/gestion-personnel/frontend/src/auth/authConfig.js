import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'gestion-personnel';
const CLIENT_SECRET = 'secret-gestion-personnel-2024';
const REDIRECT_URI = 'http://localhost:5173/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5173';
const SILENT_REDIRECT_URI = 'http://localhost:5173/silent-renew.html';

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
