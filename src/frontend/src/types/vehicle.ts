export interface Vehicle {
  id: string
  vin: string
  brand: string
  model: string
  year: number
  engineType: string
  mileage: number
  color: string
  purchaseAmount: number
  purchaseCurrency: string
  suggestedAmount: number
  suggestedCurrency: string
  transmissionType: string
  fuelConsumption: number
  engineCapacity: number
  features: string[]
  status: string
  createdAt: string
}

export interface CreateVehicleRequest {
  vin: string
  brand: string
  model: string
  year: number
  engineType: string
  mileage: number
  color: string
  purchaseAmount: number
  purchaseCurrency: string
  suggestedAmount: number
  suggestedCurrency: string
  transmissionType: string
  fuelConsumption: number
  engineCapacity: number
  features?: string[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}
