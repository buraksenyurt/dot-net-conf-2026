import axios from 'axios'
import type { Customer, PagedResult, CreateCustomerRequest } from '../types/vehicleOption'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5280/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

api.interceptors.request.use(
  (config) => { console.log(`[API Request] ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`); return config },
  (error) => { console.error('[API Request Error]', error); return Promise.reject(error) }
)

api.interceptors.response.use(
  (response) => { console.log(`[API Response] ${response.status} ${response.config.url}`); return response },
  (error) => { console.error('[API Error]', error.response?.data); return Promise.reject(error) }
)

export const customerApi = {
  async getCustomers(filters?: {
    page?: number
    pageSize?: number
    search?: string
    customerType?: string
  }): Promise<PagedResult<Customer>> {
    const params = new URLSearchParams()
    if (filters?.page) params.append('page', filters.page.toString())
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString())
    if (filters?.search) params.append('search', filters.search)
    if (filters?.customerType) params.append('customerType', filters.customerType)

    const response = await api.get<PagedResult<Customer>>(`/v1/customers?${params}`)
    return response.data
  },

  async createCustomer(request: CreateCustomerRequest): Promise<{ id: string }> {
    const response = await api.post<{ id: string }>('/v1/customers', request)
    return response.data
  }
}
