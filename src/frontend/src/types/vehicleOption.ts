export interface Customer {
  id: string
  firstName: string
  lastName: string
  email: string
  phone: string
  customerType: string
  companyName?: string
  taxNumber?: string
  createdAt: string
  updatedAt?: string
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface VehicleOption {
  id: string
  vehicleId: string
  vehicleDisplayName: string
  vehicleVIN: string
  customerId: string
  customerDisplayName: string
  expiresAt: string
  optionFeeAmount: number
  optionFeeCurrency: string
  notes?: string
  status: string
  isExpired: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateVehicleOptionRequest {
  vehicleId: string
  customerId: string
  validityDays: number
  optionFeeAmount: number
  optionFeeCurrency: string
  notes?: string
}
