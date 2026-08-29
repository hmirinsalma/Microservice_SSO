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
