export interface UserProfile {
  sub: string;
  email: string;
  email_verified?: boolean;
  name: string;
  given_name?: string;
  family_name?: string;
  roles?: string[];
  permissions?: string[];
  eams_user_id?: string;
  serviceId?: string;
}

export interface EamsContext {
  userId?: string;
  serviceId?: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: UserProfile | null;
  eamsContext: EamsContext | null;
}
