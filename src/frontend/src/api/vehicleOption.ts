import axios from 'axios'
import type { VehicleOption, CreateVehicleOptionRequest } from '../types/vehicleOption'

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

export const vehicleOptionApi = {
  async createOption(data: CreateVehicleOptionRequest): Promise<{ id: string }> {
    const response = await api.post<{ id: string }>('/vehicle-options', data)
    return response.data
  },

  async cancelOption(id: string): Promise<void> {
    await api.delete(`/vehicle-options/${id}`)
  },

  async getByVehicle(vehicleId: string): Promise<VehicleOption[]> {
    const response = await api.get<VehicleOption[]>(`/vehicle-options/vehicle/${vehicleId}`)
    return response.data
  },

  async getByCustomer(customerId: string): Promise<VehicleOption[]> {
    const response = await api.get<VehicleOption[]>(`/vehicle-options/customer/${customerId}`)
    return response.data
  }
}
