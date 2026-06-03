import type { CreateStaffPayload, StaffUser } from '../common/types/user.types';
import { apiClient } from './client';

export const adminUsersApi = {
  listStaff: async (): Promise<StaffUser[]> => {
    const { data } = await apiClient.get<StaffUser[]>('/api/admin/users/staff');
    return data;
  },

  createStaff: async (payload: CreateStaffPayload): Promise<StaffUser> => {
    const { data } = await apiClient.post<StaffUser>('/api/admin/users/staff', payload);
    return data;
  },
};
