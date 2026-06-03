import type { AuthResponse, AuthUser } from '../types/auth.types';

export const STORAGE_KEY = 'inventory_auth';

export function authResponseToUser(res: AuthResponse): AuthUser {
  return {
    userId: res.userId,
    username: res.username,
    fullName: res.fullName,
    role: res.role,
    accessToken: res.accessToken,
    refreshToken: res.refreshToken,
    accessTokenExpiresAt: res.accessTokenExpiresAt,
  };
}

export function loadStoredUser(): AuthUser | null {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as AuthUser & { token?: string };
    if (!parsed.accessToken && parsed.token) {
      parsed.accessToken = parsed.token;
    }
    return parsed;
  } catch {
    return null;
  }
}

export function saveUser(user: AuthUser): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
}

export function clearUser(): void {
  localStorage.removeItem(STORAGE_KEY);
}
