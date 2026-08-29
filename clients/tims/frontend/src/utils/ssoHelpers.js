/**
 * Utilitaires pour l'intégration SSO TIMS
 */

/**
 * Formate un custom claim TIMS pour l'affichage
 * @param {string} claimName - Nom du claim (ex: "tims_user_id")
 * @param {string|number} value - Valeur du claim
 * @returns {string} Claim formaté
 */
export const formatTimsClaim = (claimName, value) => {
  if (!value) return 'N/A';
  
  const labels = {
    tims_user_id: 'ID Utilisateur',
    tims_service_id: 'ID Service',
    tims_team_id: 'ID Équipe'
  };
  
  return `${labels[claimName] || claimName}: ${value}`;
};

/**
 * Vérifie si un token JWT est expiré
 * @param {string} token - Token JWT
 * @returns {boolean} True si expiré
 */
export const isTokenExpired = (token) => {
  if (!token) return true;
  
  try {
    const payload = parseJwt(token);
    if (!payload.exp) return true;
    
    const now = Math.floor(Date.now() / 1000);
    return payload.exp < now;
  } catch {
    return true;
  }
};

/**
 * Décode un token JWT (sans validation)
 * @param {string} token - Token JWT
 * @returns {object} Payload décodé
 */
export const parseJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('❌ Erreur lors du décodage du JWT:', error);
    return null;
  }
};

/**
 * Récupère les custom claims TIMS depuis un token
 * @param {string} token - Token JWT
 * @returns {object} Custom claims TIMS
 */
export const getTimsClaimsFromToken = (token) => {
  const payload = parseJwt(token);
  if (!payload) return null;
  
  return {
    userId: payload.tims_user_id || null,
    serviceId: payload.tims_service_id || null,
    teamId: payload.tims_team_id || null
  };
};

/**
 * Formate la durée avant expiration du token
 * @param {string} token - Token JWT
 * @returns {string} Durée formatée
 */
export const getTokenExpirationTime = (token) => {
  const payload = parseJwt(token);
  if (!payload || !payload.exp) return 'Inconnu';
  
  const now = Math.floor(Date.now() / 1000);
  const remaining = payload.exp - now;
  
  if (remaining < 0) return 'Expiré';
  
  const minutes = Math.floor(remaining / 60);
  const seconds = remaining % 60;
  
  if (minutes > 0) {
    return `${minutes}m ${seconds}s`;
  }
  return `${seconds}s`;
};

/**
 * Vérifie si l'utilisateur a un rôle spécifique
 * @param {object} user - Objet utilisateur du profil SSO
 * @param {string} role - Rôle à vérifier
 * @returns {boolean} True si l'utilisateur a le rôle
 */
export const hasRole = (user, role) => {
  if (!user || !user.roles) return false;
  return user.roles.includes(role);
};

/**
 * Vérifie si l'utilisateur a au moins un des rôles
 * @param {object} user - Objet utilisateur du profil SSO
 * @param {string[]} roles - Liste des rôles à vérifier
 * @returns {boolean} True si l'utilisateur a au moins un rôle
 */
export const hasAnyRole = (user, roles) => {
  if (!user || !user.roles) return false;
  return roles.some(role => user.roles.includes(role));
};

/**
 * Récupère le nom d'affichage de l'utilisateur
 * @param {object} user - Objet utilisateur du profil SSO
 * @returns {string} Nom d'affichage
 */
export const getUserDisplayName = (user) => {
  if (!user) return 'Utilisateur';
  return user.name || user.email || 'Utilisateur';
};

/**
 * Génère un avatar depuis les initiales
 * @param {object} user - Objet utilisateur du profil SSO
 * @returns {string} Initiales
 */
export const getUserInitials = (user) => {
  if (!user) return '?';
  
  if (user.name) {
    const parts = user.name.split(' ');
    if (parts.length >= 2) {
      return `${parts[0][0]}${parts[1][0]}`.toUpperCase();
    }
    return user.name[0].toUpperCase();
  }
  
  if (user.email) {
    return user.email[0].toUpperCase();
  }
  
  return '?';
};

/**
 * Formate la date d'expiration du token
 * @param {string} token - Token JWT
 * @returns {string} Date formatée
 */
export const getTokenExpirationDate = (token) => {
  const payload = parseJwt(token);
  if (!payload || !payload.exp) return 'Inconnue';
  
  const date = new Date(payload.exp * 1000);
  return date.toLocaleString('fr-FR');
};

/**
 * Log les informations SSO pour debug
 * @param {object} user - Objet utilisateur
 * @param {string} token - Token d'accès
 */
export const logSsoDebugInfo = (user, token) => {
  console.group('🔐 SSO Debug Info');
  console.log('User Profile:', user);
  console.log('Token:', token ? '✅ Present' : '❌ Missing');
  
  if (token) {
    const payload = parseJwt(token);
    console.log('Token Payload:', payload);
    console.log('Token Expiration:', getTokenExpirationDate(token));
    console.log('Time Remaining:', getTokenExpirationTime(token));
    console.log('TIMS Claims:', getTimsClaimsFromToken(token));
  }
  
  console.groupEnd();
};

/**
 * Nettoie le storage SSO (localStorage)
 */
export const clearSsoStorage = () => {
  const keys = Object.keys(localStorage);
  const oidcKeys = keys.filter(key => key.startsWith('oidc.'));
  
  oidcKeys.forEach(key => {
    localStorage.removeItem(key);
    console.log(`🧹 Removed: ${key}`);
  });
  
  console.log('✅ SSO storage cleared');
};

/**
 * Récupère toutes les données SSO du localStorage
 * @returns {object} Données SSO
 */
export const getSsoStorageData = () => {
  const keys = Object.keys(localStorage);
  const oidcKeys = keys.filter(key => key.startsWith('oidc.'));
  
  const data = {};
  oidcKeys.forEach(key => {
    try {
      data[key] = JSON.parse(localStorage.getItem(key));
    } catch {
      data[key] = localStorage.getItem(key);
    }
  });
  
  return data;
};

export default {
  formatTimsClaim,
  isTokenExpired,
  parseJwt,
  getTimsClaimsFromToken,
  getTokenExpirationTime,
  getTokenExpirationDate,
  hasRole,
  hasAnyRole,
  getUserDisplayName,
  getUserInitials,
  logSsoDebugInfo,
  clearSsoStorage,
  getSsoStorageData
};
