// Vehicle TypeScript interfaces
export interface Vehicle {
  id: string;
  vin: string;
  brand: string;
  model: string;
  year: number;
  engineType: EngineType;
  mileage: number;
  color: string;
  purchaseAmount: number;
  purchaseCurrency: string;
  suggestedAmount: number;
  suggestedCurrency: string;
  transmissionType: TransmissionType;
  fuelConsumption: number;
  engineCapacity: number;
  features: string[];
  status: VehicleStatus;
  createdAt: string;
}

export interface CreateVehicleRequest {
  vin: string;
  brand: string;
  model: string;
  year: number;
  engineType: string;
  mileage: number;
  color: string;
  purchaseAmount: number;
  purchaseCurrency: string;
  suggestedAmount: number;
  suggestedCurrency: string;
  transmissionType: string;
  fuelConsumption: number;
  engineCapacity: number;
  features?: string[];
}

export enum EngineType {
  Gasoline = 'Gasoline',
  Diesel = 'Diesel',
  Electric = 'Electric',
  Hybrid = 'Hybrid'
}

export enum TransmissionType {
  Manual = 'Manual',
  Automatic = 'Automatic',
  SemiAutomatic = 'SemiAutomatic'
}

export enum VehicleStatus {
  InStock = 'InStock',
  OnSale = 'OnSale',
  Sold = 'Sold',
  Reserved = 'Reserved'
}

export interface VehicleFilters {
  page?: number;
  pageSize?: number;
  brand?: string;
  status?: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
