import axios from 'axios'
import type { ServiceAdvisor, LoginRequest } from '../types/serviceAdvisor'
import type { VehicleOption } from '../types/vehicleOption'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5280/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

api.interceptors.response.use(
  (response) => response,
  (error) => { console.error('[SA API Error]', error.response?.data); return Promise.reject(error) }
)

export const serviceAdvisorApi = {
  async login(data: LoginRequest): Promise<ServiceAdvisor> {
    const response = await api.post<ServiceAdvisor>('/service-advisors/login', data)
    return response.data
  },

  async getDashboard(advisorId: string): Promise<VehicleOption[]> {
    const response = await api.get<VehicleOption[]>(`/service-advisors/${advisorId}/dashboard`)
    return response.data
  }
}
