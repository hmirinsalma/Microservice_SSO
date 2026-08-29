import { UserManager, WebStorageStateStore, UserManagerSettings } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'eams-spa';
const CLIENT_SECRET = 'secret-eams-2024';
const REDIRECT_URI = 'http://localhost:5174/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5174';
const SILENT_REDIRECT_URI = 'http://localhost:5174/silent-renew.html';

export const oidcConfig: UserManagerSettings = {
  authority: AUTHORITY,
  client_id: CLIENT_ID,
  client_secret: CLIENT_SECRET,
  redirect_uri: REDIRECT_URI,
  post_logout_redirect_uri: POST_LOGOUT_REDIRECT_URI,
  silent_redirect_uri: SILENT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email roles offline_access eams eams_user_id serviceId',
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
  console.log('✅ EAMS User loaded:', user.profile);
  console.log('📋 Custom claims:', {
    eams_user_id: user.profile.eams_user_id,
    serviceId: user.profile.serviceId
  });
});

userManager.events.addUserUnloaded(() => {
  console.log('🚪 EAMS User logged out');
});

userManager.events.addAccessTokenExpired(() => {
  console.log('⏱️ Access token expired');
  userManager.signinSilent();
});

userManager.events.addSilentRenewError((error) => {
  console.error('❌ Silent renew error:', error);
});
