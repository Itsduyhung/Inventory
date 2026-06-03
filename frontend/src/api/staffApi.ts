import type { PagedResult } from '../common/types/paged.types';
import type { SaleRecord, SaleRecordDetail } from '../common/types/sale.types';
import type { Vehicle, VehicleDetail, VehicleFilter } from '../common/types/vehicle.types';
import { apiClient } from './client';

export const staffApi = {
  getInventory: async (filter: VehicleFilter = {}): Promise<PagedResult<Vehicle>> => {
    const { data } = await apiClient.get<PagedResult<Vehicle>>('/api/staff/inventory', {
      params: filter,
    });
    return data;
  },

  getVehicleDetail: async (id: string): Promise<VehicleDetail> => {
    const { data } = await apiClient.get<VehicleDetail>(`/api/staff/inventory/${id}`);
    return data;
  },

  processSale: async (id: string): Promise<Vehicle> => {
    const { data } = await apiClient.post<Vehicle>(`/api/staff/inventory/${id}/sale`);
    return data;
  },

  getHistory: async (page = 1, pageSize = 10): Promise<PagedResult<SaleRecord>> => {
    const { data } = await apiClient.get<PagedResult<SaleRecord>>('/api/staff/history', {
      params: { page, pageSize },
    });
    return data;
  },

  getHistoryDetail: async (id: string): Promise<SaleRecordDetail> => {
    const { data } = await apiClient.get<SaleRecordDetail>(`/api/staff/history/${id}`);
    return data;
  },
};
