import axios from 'axios'
import type { Vehicle, CreateVehicleRequest, PagedResult } from '../types/vehicle'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5280/api'

console.log('[API Config] Base URL:', API_BASE)
console.log('[API Config] Environment:', import.meta.env.MODE)

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

// Add request/response interceptors for debugging
api.interceptors.request.use(
  (config) => {
    console.log(`[API Request] ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`)
    return config
  },
  (error) => {
    console.error('[API Request Error]', error)
    return Promise.reject(error)
  }
)

api.interceptors.response.use(
  (response) => {
    console.log(`[API Response] ${response.status} ${response.config.url}`)
    return response
  },
  (error) => {
    console.error('[API Error]', {
      message: error.message,
      code: error.code,
      url: error.config?.url,
      status: error.response?.status,
      data: error.response?.data
    })
    return Promise.reject(error)
  }
)

export const vehicleApi = {
  async getVehicles(filters?: {
    page?: number
    pageSize?: number
    brand?: string
    status?: string
  }): Promise<PagedResult<Vehicle>> {
    const params = new URLSearchParams()
    if (filters?.page) params.append('page', filters.page.toString())
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString())
    if (filters?.brand) params.append('brand', filters.brand)
    if (filters?.status) params.append('status', filters.status)

    const response = await api.get<PagedResult<Vehicle>>(`/v1/vehicles?${params}`)
    return response.data
  },

  async createVehicle(data: CreateVehicleRequest): Promise<{ id: string }> {
    const response = await api.post<{ id: string }>('/v1/vehicles', data)
    return response.data
  }
}
