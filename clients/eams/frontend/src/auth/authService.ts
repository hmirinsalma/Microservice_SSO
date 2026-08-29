import { User } from 'oidc-client-ts';
import { userManager } from './authConfig';
import { UserProfile, EamsContext } from './types';

class AuthService {
  login(): Promise<void> {
    return userManager.signinRedirect();
  }

  async completeLogin(): Promise<User> {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  logout(): Promise<void> {
    return userManager.signoutRedirect();
  }

  async getUser(): Promise<User | null> {
    return await userManager.getUser();
  }

  async isAuthenticated(): Promise<boolean> {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  async getAccessToken(): Promise<string | undefined> {
    const user = await this.getUser();
    return user?.access_token;
  }

  async getUserProfile(): Promise<UserProfile | null> {
    const user = await this.getUser();
    return user?.profile as UserProfile | null;
  }

  async getUserRoles(): Promise<string[]> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.roles || [];
  }

  async getUserPermissions(): Promise<string[]> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.permissions || [];
  }

  // ⭐ CUSTOM: Get EAMS User ID
  async getEamsUserId(): Promise<string | undefined> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.eams_user_id;
  }

  // ⭐ CUSTOM: Get Service ID
  async getServiceId(): Promise<string | undefined> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.serviceId;
  }

  // ⭐ CUSTOM: Get all EAMS custom claims
  async getEamsContext(): Promise<EamsContext> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return {
      userId: profile?.eams_user_id,
      serviceId: profile?.serviceId
    };
  }

  async hasRole(role: string): Promise<boolean> {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  async hasPermission(permission: string): Promise<boolean> {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
