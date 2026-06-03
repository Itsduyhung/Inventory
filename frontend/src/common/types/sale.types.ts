import type { VehicleDetail } from './vehicle.types';

export interface SaleRecord {
  id: string;
  vehicleId: string;
  vehicleTitle: string;
  vin: string;
  quantitySold: number;
  salePrice: number;
  remainingQuantityAfterSale: number;
  soldAt: string;
  note: string | null;
  staffName: string;
  staffUserId: string;
  staffUsername: string;
  vehicleImageUrl?: string | null;
}

export interface SaleRecordDetail {
  id: string;
  quantitySold: number;
  salePrice: number;
  remainingQuantityAfterSale: number;
  soldAt: string;
  note: string | null;
  staffName: string;
  vehicle: VehicleDetail;
}
