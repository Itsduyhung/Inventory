import type {
  AuthResponse,
  ForgotPasswordRequest,
  LoginRequest,
  MessageResponse,
  ResetPasswordRequest,
} from '../common/types/auth.types';
import { apiClient } from './client';

export const authApi = {
  login: async (payload: LoginRequest): Promise<AuthResponse> => {
    const { data } = await apiClient.post<AuthResponse>('/api/auth/login', payload);
    return data;
  },

  refresh: async (refreshToken: string): Promise<AuthResponse> => {
    const { data } = await apiClient.post<AuthResponse>('/api/auth/refresh', { refreshToken });
    return data;
  },

  forgotPassword: async (payload: ForgotPasswordRequest): Promise<MessageResponse> => {
    const { data } = await apiClient.post<MessageResponse>('/api/auth/forgot-password', payload);
    return data;
  },

  resetPassword: async (payload: ResetPasswordRequest): Promise<MessageResponse> => {
    const { data } = await apiClient.post<MessageResponse>('/api/auth/reset-password', payload);
    return data;
  },
};
