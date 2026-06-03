import type { PagedResult } from '../common/types/paged.types';
import type {
  CreateVehiclePayload,
  InventorySummary,
  UpdateVehiclePayload,
  Vehicle,
  VehicleDetail,
  VehicleFilter,
} from '../common/types/vehicle.types';
import { apiClient } from './client';

export const adminVehicleApi = {
  getSummary: async (): Promise<InventorySummary> => {
    const { data } = await apiClient.get<InventorySummary>('/api/admin/vehicles/summary');
    return data;
  },

  getAll: async (filter: VehicleFilter): Promise<PagedResult<Vehicle>> => {
    const { data } = await apiClient.get<PagedResult<Vehicle>>('/api/admin/vehicles', {
      params: { ...filter, includeDisabled: true },
    });
    return data;
  },

  getById: async (id: string): Promise<VehicleDetail> => {
    const { data } = await apiClient.get<VehicleDetail>(`/api/admin/vehicles/${id}`);
    return data;
  },

  create: async (payload: CreateVehiclePayload): Promise<VehicleDetail> => {
    const { data } = await apiClient.post<VehicleDetail>('/api/admin/vehicles', payload);
    return data;
  },

  update: async (id: string, payload: UpdateVehiclePayload): Promise<VehicleDetail> => {
    const { data } = await apiClient.put<VehicleDetail>(`/api/admin/vehicles/${id}`, payload);
    return data;
  },

  disable: async (id: string): Promise<VehicleDetail> => {
    const { data } = await apiClient.patch<VehicleDetail>(`/api/admin/vehicles/${id}/disable`);
    return data;
  },

  addQuantity: async (id: string, amount = 1): Promise<VehicleDetail> => {
    const { data } = await apiClient.patch<VehicleDetail>(
      `/api/admin/vehicles/${id}/add-quantity`,
      { amount },
    );
    return data;
  },
};
