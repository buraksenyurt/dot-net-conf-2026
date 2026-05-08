import { ref, reactive } from 'vue'
import { VehicleOptionStatus } from '../types/vehicleOption'
import type { VehicleOptionSummaryDto, VehicleOptionSummaryQuery, PagedResult } from '../types/vehicleOption'
import { vehicleOptionApi } from '../api/vehicleOption'

export function useVehicleOptionSummary() {
  const loading = ref(false)
  const error = ref<string | null>(null)
  const result = ref<PagedResult<VehicleOptionSummaryDto> | null>(null)

  const query = reactive<VehicleOptionSummaryQuery>({
    customerSearch: '',
    vehicleSearch: '',
    status: null,
    createdFrom: null,
    createdTo: null,
    page: 1,
    pageSize: 20,
    sortBy: 'expiresAt',
    sortDirection: 'asc'
  })

  const load = async () => {
    loading.value = true
    error.value = null
    try {
      result.value = await vehicleOptionApi.getSummary({ ...query })
    } catch {
      error.value = 'Opsiyonlama kayıtları yüklenirken bir hata oluştu. Lütfen tekrar deneyin.'
    } finally {
      loading.value = false
    }
  }

  const applyFilters = () => {
    query.page = 1
    load()
  }

  const resetFilters = () => {
    query.customerSearch = ''
    query.vehicleSearch = ''
    query.status = null
    query.createdFrom = null
    query.createdTo = null
    query.page = 1
    query.pageSize = 20
    query.sortBy = 'expiresAt'
    query.sortDirection = 'asc'
    load()
  }

  const goToPage = (page: number) => {
    query.page = page
    load()
  }

  const setSort = (sortBy: string) => {
    if (query.sortBy === sortBy) {
      query.sortDirection = query.sortDirection === 'asc' ? 'desc' : 'asc'
    } else {
      query.sortBy = sortBy
      query.sortDirection = 'asc'
    }
    query.page = 1
    load()
  }

  const getSortIcon = (col: string): string => {
    if (query.sortBy !== col) return 'bi-arrow-down-up text-muted'
    return query.sortDirection === 'asc' ? 'bi-arrow-up text-primary' : 'bi-arrow-down text-primary'
  }

  const getAriaSort = (col: string): 'ascending' | 'descending' | 'none' => {
    if (query.sortBy !== col) return 'none'
    return query.sortDirection === 'asc' ? 'ascending' : 'descending'
  }

  // AC-3 / BR-2: isExpired flag veya status=Active + ExpiresAt < now ise Expired olarak göster
  const getDisplayStatus = (item: VehicleOptionSummaryDto): 'active' | 'expired' | 'cancelled' => {
    if (item.status === VehicleOptionStatus.Cancelled) return 'cancelled'
    if (item.isExpired || item.status === VehicleOptionStatus.Expired) return 'expired'
    return 'active'
  }

  const getStatusBadgeClass = (item: VehicleOptionSummaryDto): string => {
    const s = getDisplayStatus(item)
    if (s === 'active') return 'badge bg-success'
    if (s === 'expired') return 'badge bg-warning text-dark'
    return 'badge bg-secondary'
  }

  const getStatusLabel = (item: VehicleOptionSummaryDto): string => {
    const s = getDisplayStatus(item)
    if (s === 'active') return 'Aktif'
    if (s === 'expired') return 'Süresi Dolmuş'
    return 'İptal Edilmiş'
  }

  const formatDate = (iso: string): string => {
    if (!iso) return '—'
    const d = new Date(iso)
    return d.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' })
  }

  const formatCurrency = (amount: number, currency: string): string => {
    return new Intl.NumberFormat('tr-TR', {
      style: 'currency',
      currency: currency || 'TRY',
      minimumFractionDigits: 2
    }).format(amount)
  }

  return {
    loading,
    error,
    result,
    query,
    load,
    applyFilters,
    resetFilters,
    goToPage,
    setSort,
    getSortIcon,
    getAriaSort,
    getStatusBadgeClass,
    getStatusLabel,
    formatDate,
    formatCurrency
  }
}
