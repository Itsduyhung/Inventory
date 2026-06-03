import type { CreateVehiclePayload } from '../types/vehicle.types';

export type FormErrors = Partial<Record<keyof VehicleFormValues | 'form', string>>;

export interface VehicleFormValues extends CreateVehiclePayload {
  priceText: string;
  yearText: string;
  quantityText: string;
  mileageText: string;
}

export function todayDateInputValue(): string {
  return new Date().toISOString().split('T')[0];
}

export function emptyVehicleFormValues(): VehicleFormValues {
  return {
    make: '',
    model: '',
    year: new Date().getFullYear(),
    yearText: String(new Date().getFullYear()),
    vin: '',
    price: 0,
    priceText: '',
    quantity: 1,
    quantityText: '1',
    note: '',
    dateAddedToInventory: todayDateInputValue(),
    color: '',
    exteriorColor: '',
    interiorColor: '',
    mileage: 0,
    mileageText: '0',
    fuelType: 'Gasoline',
    transmission: 'Automatic',
    engine: '',
    bodyType: 'Sedan',
    description: '',
    imageUrl: '',
  };
}

export function validateVehicleForm(values: VehicleFormValues): FormErrors {
  const errors: FormErrors = {};

  if (!values.make.trim()) errors.make = 'Make is required.';
  else if (values.make.trim().length < 2) errors.make = 'Make must be at least 2 characters.';

  if (!values.model.trim()) errors.model = 'Model is required.';
  else if (values.model.trim().length < 1) errors.model = 'Model is required.';

  const year = parseInt(values.yearText, 10);
  if (!values.yearText.trim() || Number.isNaN(year)) errors.yearText = 'Year is required.';
  else if (year < 1980 || year > new Date().getFullYear() + 2) {
    errors.yearText = `Year must be between 1980 and ${new Date().getFullYear() + 2}.`;
  }

  const vin = values.vin.trim().toUpperCase();
  if (!vin) errors.vin = 'VIN is required.';
  else if (vin.length < 11 || vin.length > 17) errors.vin = 'VIN must be 11–17 characters.';

  const price = parseFloat(values.priceText.replace(/,/g, ''));
  if (!values.priceText.trim() || Number.isNaN(price)) errors.priceText = 'Price is required.';
  else if (price <= 0) errors.priceText = 'Price must be greater than 0.';

  const quantity = parseInt(values.quantityText, 10);
  if (!values.quantityText.trim() || Number.isNaN(quantity)) errors.quantityText = 'Quantity is required.';
  else if (quantity < 0) errors.quantityText = 'Quantity cannot be negative.';

  const mileage = parseInt(values.mileageText.replace(/,/g, ''), 10);
  if (!values.mileageText.trim() || Number.isNaN(mileage)) errors.mileageText = 'Mileage is required.';
  else if (mileage < 0) errors.mileageText = 'Mileage cannot be negative.';

  if (values.dateAddedToInventory) {
    const selected = new Date(`${values.dateAddedToInventory}T00:00:00`);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (selected > today) {
      errors.dateAddedToInventory = 'Date added cannot be in the future.';
    }
  } else {
    errors.dateAddedToInventory = 'Date added is required.';
  }

  if (!values.description.trim()) errors.description = 'Description is required.';
  else if (values.description.trim().length < 10) {
    errors.description = 'Description must be at least 10 characters.';
  }

  return errors;
}

export function formValuesToPayload(values: VehicleFormValues): CreateVehiclePayload {
  return {
    make: values.make.trim(),
    model: values.model.trim(),
    year: parseInt(values.yearText, 10),
    vin: values.vin.trim().toUpperCase(),
    price: parseFloat(values.priceText.replace(/,/g, '')),
    quantity: parseInt(values.quantityText, 10),
    note: (values.note ?? '').trim() || undefined,
    dateAddedToInventory: values.dateAddedToInventory || new Date().toISOString().split('T')[0],
    color: values.color.trim() || 'N/A',
    exteriorColor: values.exteriorColor.trim() || values.color.trim() || 'N/A',
    interiorColor: values.interiorColor.trim() || 'N/A',
    mileage: parseInt(values.mileageText.replace(/,/g, ''), 10),
    fuelType: values.fuelType.trim() || 'Gasoline',
    transmission: values.transmission.trim() || 'Automatic',
    engine: values.engine.trim() || 'N/A',
    bodyType: values.bodyType.trim() || 'Sedan',
    description: values.description.trim(),
    imageUrl: (values.imageUrl ?? '').trim() || undefined,
  };
}

export function vehicleDetailToFormValues(vehicle: {
  make: string;
  model: string;
  year: number;
  vin: string;
  price: number;
  quantity: number;
  note: string | null;
  dateAddedToInventory: string;
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
}): VehicleFormValues {
  return {
    make: vehicle.make,
    model: vehicle.model,
    year: vehicle.year,
    yearText: String(vehicle.year),
    vin: vehicle.vin,
    price: vehicle.price,
    priceText: String(vehicle.price),
    quantity: vehicle.quantity,
    quantityText: String(vehicle.quantity),
    note: vehicle.note ?? '',
    dateAddedToInventory: vehicle.dateAddedToInventory.split('T')[0],
    color: vehicle.color,
    exteriorColor: vehicle.exteriorColor,
    interiorColor: vehicle.interiorColor,
    mileage: vehicle.mileage,
    mileageText: String(vehicle.mileage),
    fuelType: vehicle.fuelType,
    transmission: vehicle.transmission,
    engine: vehicle.engine,
    bodyType: vehicle.bodyType,
    description: vehicle.description,
    imageUrl: vehicle.imageUrl ?? '',
  };
}
