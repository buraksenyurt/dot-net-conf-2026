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
  serviceAdvisorId?: string
  serviceAdvisorDisplayName?: string
}

export interface CreateVehicleOptionRequest {
  vehicleId: string
  customerId: string
  validityDays: number
  optionFeeAmount: number
  optionFeeCurrency: string
  serviceAdvisorId?: string
  notes?: string
}

export interface CreateCustomerRequest {
  firstName: string
  lastName: string
  email: string
  phone: string
  customerType: string
  companyName?: string
  taxNumber?: string
}

// US-007 ─────────────────────────────────────────────────────────────────────

export const VehicleOptionStatus = {
  Active: 1,
  Expired: 2,
  Cancelled: 3
} as const

export type VehicleOptionStatus = (typeof VehicleOptionStatus)[keyof typeof VehicleOptionStatus]

export interface VehicleOptionSummaryDto {
  id: string
  vehicleId: string
  vehicleDisplayName: string
  vehicleVIN: string
  customerId: string
  customerDisplayName: string
  serviceAdvisorId: string
  serviceAdvisorDisplayName: string
  expiresAt: string
  optionFeeAmount: number
  optionFeeCurrency: string
  notes?: string
  status: VehicleOptionStatus
  isExpired: boolean
  createdAt: string
}

export interface VehicleOptionSummaryQuery {
  customerSearch?: string
  vehicleSearch?: string
  status?: VehicleOptionStatus | null
  createdFrom?: string | null
  createdTo?: string | null
  page: number
  pageSize: number
  sortBy: string
  sortDirection: 'asc' | 'desc'
}
