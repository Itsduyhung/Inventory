import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import type {
  AuthUser,
  ForgotPasswordRequest,
  LoginRequest,
  ResetPasswordRequest,
} from '../types/auth.types';
import { authApi } from '../../api/authApi';
import {
  authResponseToUser,
  clearUser,
  loadStoredUser,
  saveUser,
} from '../auth/authSession';

interface AuthContextValue {
  user: AuthUser | null;
  login: (payload: LoginRequest) => Promise<void>;
  forgotPassword: (payload: ForgotPasswordRequest) => Promise<{ message: string; resetToken?: string | null }>;
  resetPassword: (payload: ResetPasswordRequest) => Promise<string>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(loadStoredUser);

  const applySession = useCallback((res: Awaited<ReturnType<typeof authApi.login>>) => {
    const authUser = authResponseToUser(res);
    saveUser(authUser);
    setUser(authUser);
  }, []);

  const login = useCallback(async (payload: LoginRequest) => {
    applySession(await authApi.login(payload));
  }, [applySession]);

  const forgotPassword = useCallback(async (payload: ForgotPasswordRequest) => {
    const res = await authApi.forgotPassword(payload);
    return { message: res.message, resetToken: res.resetToken };
  }, []);

  const resetPassword = useCallback(async (payload: ResetPasswordRequest) => {
    const res = await authApi.resetPassword(payload);
    return res.message;
  }, []);

  const logout = useCallback(() => {
    clearUser();
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, login, forgotPassword, resetPassword, logout, isAuthenticated: !!user }),
    [user, login, forgotPassword, resetPassword, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
