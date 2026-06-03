import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { API_BASE_URL } from '../common/constants';
import type { AuthResponse } from '../common/types/auth.types';
import { authResponseToUser, clearUser, loadStoredUser, saveUser } from '../common/auth/authSession';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

let refreshPromise: Promise<string | null> | null = null;

async function refreshAccessToken(): Promise<string | null> {
  const user = loadStoredUser();
  if (!user?.refreshToken) return null;

  try {
    const { data } = await axios.post<AuthResponse>(`${API_BASE_URL}/api/auth/refresh`, {
      refreshToken: user.refreshToken,
    });
    const next = authResponseToUser(data);
    saveUser(next);
    return next.accessToken;
  } catch {
    clearUser();
    return null;
  }
}

apiClient.interceptors.request.use((config) => {
  const user = loadStoredUser();
  if (user?.accessToken) {
    config.headers.Authorization = `Bearer ${user.accessToken}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
    const isAuthRoute = original?.url?.includes('/api/auth/');

    if (
      error.response?.status !== 401
      || !original
      || original._retry
      || isAuthRoute
    ) {
      return Promise.reject(error);
    }

    original._retry = true;

    if (!refreshPromise) {
      refreshPromise = refreshAccessToken().finally(() => {
        refreshPromise = null;
      });
    }

    const token = await refreshPromise;
    if (!token) {
      clearUser();
      window.location.href = '/login';
      return Promise.reject(error);
    }

    original.headers.Authorization = `Bearer ${token}`;
    return apiClient(original);
  },
);
