import { ref, computed } from 'vue'
import type { ServiceAdvisor } from '../types/serviceAdvisor'

const STORAGE_KEY = 'aio_sa_session'

// Module-level singleton — shared across all components
const _advisor = ref<ServiceAdvisor | null>(
  JSON.parse(localStorage.getItem(STORAGE_KEY) || 'null')
)

export function useAuth() {
  function setAdvisor(advisor: ServiceAdvisor) {
    _advisor.value = advisor
    localStorage.setItem(STORAGE_KEY, JSON.stringify(advisor))
  }

  function logout() {
    _advisor.value = null
    localStorage.removeItem(STORAGE_KEY)
  }

  const isLoggedIn = computed(() => _advisor.value !== null)

  return {
    advisor: _advisor,
    isLoggedIn,
    setAdvisor,
    logout
  }
}
