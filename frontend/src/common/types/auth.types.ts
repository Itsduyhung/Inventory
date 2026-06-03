export type UserRole = 'Admin' | 'Staff';

export interface AuthUser {
  userId: string;
  username: string;
  fullName: string;
  role: UserRole;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface ForgotPasswordRequest {
  usernameOrEmail: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  userId: string;
  username: string;
  fullName: string;
  role: UserRole;
}

export interface MessageResponse {
  message: string;
  resetToken?: string | null;
}
