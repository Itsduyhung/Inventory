export interface Vehicle {
  id: string;
  make: string;
  model: string;
  year: number;
  vin: string;
  price: number;
  quantity: number;
  isDisabled: boolean;
  note: string | null;
  dateAddedToInventory: string;
  daysInInventory: number;
  isAgingStock: boolean;
  isLowStock: boolean;
  color: string;
  exteriorColor: string;
  interiorColor: string;
  mileage: number;
  fuelType: string;
  transmission: string;
  engine: string;
  bodyType: string;
  description: string;
  imageUrl: string | null;
}

export interface VehicleDetail extends Vehicle {
  createdAt: string;
  updatedAt: string | null;
}

export interface VehicleFilter {
  make?: string;
  model?: string;
  minAgeDays?: number;
  maxAgeDays?: number;
  agingOnly?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface InventorySummary {
  totalVehicles: number;
  agingStockCount: number;
  vehiclesWithNote: number;
}

export interface CreateVehiclePayload {
  make: string;
  model: string;
  year: number;
  vin: string;
  price: number;
  quantity: number;
  note?: string;
  dateAddedToInventory?: string;
  color: string;
  exteriorColor: string;
  interiorColor: string;
  mileage: number;
  fuelType: string;
  transmission: string;
  engine: string;
  bodyType: string;
  description: string;
  imageUrl?: string;
}

export type UpdateVehiclePayload = Omit<CreateVehiclePayload, 'dateAddedToInventory'> & {
  dateAddedToInventory: string;
};
