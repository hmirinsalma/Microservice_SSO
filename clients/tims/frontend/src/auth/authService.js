import { userManager } from './authConfig';

class AuthService {
  login() {
    return userManager.signinRedirect();
  }

  async completeLogin() {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  logout() {
    return userManager.signoutRedirect();
  }

  async getUser() {
    return await userManager.getUser();
  }

  async isAuthenticated() {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  async getAccessToken() {
    const user = await this.getUser();
    return user?.access_token;
  }

  async getUserProfile() {
    const user = await this.getUser();
    return user?.profile;
  }

  async getUserRoles() {
    const user = await this.getUser();
    return user?.profile?.roles || [];
  }

  async getUserPermissions() {
    const user = await this.getUser();
    return user?.profile?.permissions || [];
  }

  // ⭐ CUSTOM: Get TIMS User ID
  async getTimsUserId() {
    const user = await this.getUser();
    return user?.profile?.tims_user_id;
  }

  // ⭐ CUSTOM: Get TIMS Service ID
  async getTimsServiceId() {
    const user = await this.getUser();
    return user?.profile?.tims_service_id;
  }

  // ⭐ CUSTOM: Get TIMS Team ID
  async getTimsTeamId() {
    const user = await this.getUser();
    return user?.profile?.tims_team_id;
  }

  // ⭐ CUSTOM: Get all TIMS custom claims
  async getTimsContext() {
    const user = await this.getUser();
    return {
      userId: user?.profile?.tims_user_id,
      serviceId: user?.profile?.tims_service_id,
      teamId: user?.profile?.tims_team_id
    };
  }

  async hasRole(role) {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  async hasPermission(permission) {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
