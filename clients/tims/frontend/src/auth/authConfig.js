import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'tims-app';
const CLIENT_SECRET = 'secret-tims-2024';
const REDIRECT_URI = 'http://localhost:5175/auth/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5175';
const SILENT_REDIRECT_URI = 'http://localhost:5175/silent-renew.html';

export const oidcConfig = {
  authority: AUTHORITY,
  client_id: CLIENT_ID,
  client_secret: CLIENT_SECRET,
  redirect_uri: REDIRECT_URI,
  post_logout_redirect_uri: POST_LOGOUT_REDIRECT_URI,
  silent_redirect_uri: SILENT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email roles offline_access tims tims_user_id tims_service_id tims_team_id',
  automaticSilentRenew: false,
  loadUserInfo: false,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  metadata: {
    issuer: 'ONEE.SSO',
    authorization_endpoint: `${AUTHORITY}/connect/authorize`,
    token_endpoint: `${AUTHORITY}/connect/token`,
    userinfo_endpoint: `${AUTHORITY}/api/auth/userinfo`,
    end_session_endpoint: `${AUTHORITY}/connect/logout`,
    jwks_uri: `${AUTHORITY}/.well-known/jwks.json`,
  }
};

export const userManager = new UserManager(oidcConfig);

userManager.events.addUserLoaded((user) => {
  console.log('✅ TIMS User loaded:', user.profile);
  console.log('📋 Custom claims:', {
    tims_user_id: user.profile.tims_user_id,
    tims_service_id: user.profile.tims_service_id,
    tims_team_id: user.profile.tims_team_id
  });
});

userManager.events.addUserUnloaded(() => {
  console.log('🚪 TIMS User logged out');
});

userManager.events.addAccessTokenExpired(() => {
  console.log('⏱️ Access token expired');
  userManager.signinSilent();
});

userManager.events.addSilentRenewError((error) => {
  console.error('❌ Silent renew error:', error);
});
