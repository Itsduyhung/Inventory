import type { PagedResult } from '../common/types/paged.types';
import type { SaleRecord, SaleRecordDetail } from '../common/types/sale.types';
import { apiClient } from './client';

export interface AdminSalesFilter {
  page?: number;
  pageSize?: number;
  staffUserId?: string;
}

export const adminSalesApi = {
  getSales: async (filter: AdminSalesFilter = {}): Promise<PagedResult<SaleRecord>> => {
    const { data } = await apiClient.get<PagedResult<SaleRecord>>('/api/admin/sales', {
      params: filter,
    });
    return data;
  },

  getSaleDetail: async (id: string): Promise<SaleRecordDetail> => {
    const { data } = await apiClient.get<SaleRecordDetail>(`/api/admin/sales/${id}`);
    return data;
  },
};
