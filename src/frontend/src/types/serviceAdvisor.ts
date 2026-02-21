export interface ServiceAdvisor {
  id: string
  firstName: string
  lastName: string
  email: string
  department: string
  isActive: boolean
}

export interface LoginRequest {
  email: string
  password: string
}
