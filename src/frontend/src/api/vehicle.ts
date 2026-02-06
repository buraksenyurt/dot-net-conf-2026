import axios from 'axios'
import type { Vehicle, CreateVehicleRequest, PagedResult } from '../types/vehicle'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
})

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

    const response = await api.get<PagedResult<Vehicle>>(+"/v1/vehicles?${params}"+)
    return response.data
  },

  async createVehicle(data: CreateVehicleRequest): Promise<{ id: string }> {
    const response = await api.post<{ id: string }>('/v1/vehicles', data)
    return response.data
  }
}
