<template>
  <div>
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-1">
          <i class="bi bi-bookmark-star me-2 text-primary"></i>
          Opsiyonlar
        </h2>
        <p class="text-muted">Araç veya müşteriye göre opsiyon geçmişini sorgulayın</p>
      </div>
      <div class="col-auto d-flex align-items-center">
        <router-link to="/vehicle-options/new" class="btn btn-success">
          <i class="bi bi-bookmark-plus-fill me-2"></i>Yeni Opsiyon
        </router-link>
      </div>
    </div>

    <!-- Alert -->
    <div v-if="alertMessage" :class="`alert alert-${alertType} d-flex align-items-center mb-4`" role="alert">
      <i :class="`bi ${alertType === 'success' ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill'} flex-shrink-0 me-2`"></i>
      <div>{{ alertMessage }}</div>
    </div>

    <!-- Tabs -->
    <ul class="nav nav-tabs mb-4">
      <li class="nav-item">
        <button
          class="nav-link"
          :class="{ active: activeTab === 'vehicle' }"
          @click="switchTab('vehicle')"
        >
          <i class="bi bi-car-front me-2"></i>Araca Göre
        </button>
      </li>
      <li class="nav-item">
        <button
          class="nav-link"
          :class="{ active: activeTab === 'customer' }"
          @click="switchTab('customer')"
        >
          <i class="bi bi-person me-2"></i>Müşteriye Göre
        </button>
      </li>
    </ul>

    <!-- Search by Vehicle -->
    <div v-if="activeTab === 'vehicle'" class="card shadow-sm border-0 mb-4">
      <div class="card-body">
        <div class="row g-3 align-items-end">
          <div class="col-md-8">
            <label class="form-label fw-semibold">
              <i class="bi bi-search me-1"></i>Araç Ara
            </label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-car-front"></i></span>
              <input
                v-model="vehicleSearch"
                type="text"
                class="form-control"
                placeholder="Marka veya model ile ara..."
                @input="onVehicleSearch"
                @focus="showVehicleDropdown = vehicleResults.length > 0"
              />
              <button v-if="vehicleSearch" class="btn btn-outline-secondary" type="button" @click="clearVehicleSearch">
                <i class="bi bi-x"></i>
              </button>
            </div>
            <!-- Dropdown -->
            <div v-if="showVehicleDropdown && vehicleResults.length > 0"
              class="list-group border shadow-sm mt-1 position-absolute z-3"
              style="max-height:200px;overflow-y:auto;min-width:300px"
            >
              <button
                v-for="v in vehicleResults"
                :key="v.id"
                type="button"
                class="list-group-item list-group-item-action py-2"
                @click="selectVehicleForFilter(v)"
              >
                <div class="d-flex justify-content-between align-items-center">
                  <div>
                    <strong>{{ v.brand }} {{ v.model }} {{ v.year }}</strong>
                    <small class="d-block text-muted"><code>{{ v.vin }}</code></small>
                  </div>
                  <span :class="getStatusClass(v.status)" class="badge ms-2">{{ getStatusText(v.status) }}</span>
                </div>
              </button>
            </div>
            <div v-if="vehicleSearchLoading" class="form-text">
              <span class="spinner-border spinner-border-sm me-1"></span>Aranıyor...
            </div>
          </div>
          <div class="col-md-4">
            <button class="btn btn-primary w-100" :disabled="!selectedVehicleId || vehicleLoading" @click="loadByVehicle">
              <span v-if="vehicleLoading" class="spinner-border spinner-border-sm me-2"></span>
              <i v-else class="bi bi-search me-2"></i>
              Opsiyon Geçmişini Getir
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Search by Customer -->
    <div v-if="activeTab === 'customer'" class="card shadow-sm border-0 mb-4">
      <div class="card-body">
        <div class="row g-3 align-items-end">
          <div class="col-md-8">
            <label class="form-label fw-semibold">
              <i class="bi bi-search me-1"></i>Müşteri Ara
            </label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-person"></i></span>
              <input
                v-model="customerSearch"
                type="text"
                class="form-control"
                placeholder="Ad, soyad veya e-posta ile ara..."
                @input="onCustomerSearch"
                @focus="showCustomerDropdown = customerResults.length > 0"
              />
              <button v-if="customerSearch" class="btn btn-outline-secondary" type="button" @click="clearCustomerSearch">
                <i class="bi bi-x"></i>
              </button>
            </div>
            <!-- Dropdown -->
            <div
              v-if="showCustomerDropdown && customerResults.length > 0"
              class="list-group border shadow-sm mt-1 position-absolute z-3"
              style="max-height:200px;overflow-y:auto;min-width:300px"
            >
              <button
                v-for="c in customerResults"
                :key="c.id"
                type="button"
                class="list-group-item list-group-item-action py-2"
                @click="selectCustomerForFilter(c)"
              >
                <strong>{{ c.firstName }} {{ c.lastName }}</strong>
                <small class="d-block text-muted">{{ c.email }}</small>
              </button>
            </div>
            <div v-if="customerSearchLoading" class="form-text">
              <span class="spinner-border spinner-border-sm me-1"></span>Aranıyor...
            </div>
          </div>
          <div class="col-md-4">
            <button class="btn btn-primary w-100" :disabled="!selectedCustomerId || customerLoading" @click="loadByCustomer">
              <span v-if="customerLoading" class="spinner-border spinner-border-sm me-2"></span>
              <i v-else class="bi bi-search me-2"></i>
              Opsiyonları Getir
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" style="width:2.5rem;height:2.5rem;"></div>
      <p class="mt-3 text-muted">Yükleniyor...</p>
    </div>

    <!-- Results Table -->
    <div v-else-if="options.length > 0" class="card shadow-sm border-0">
      <div class="card-header bg-white d-flex justify-content-between align-items-center">
        <span class="fw-semibold">
          <i class="bi bi-list-ul me-2 text-primary"></i>
          {{ options.length }} opsiyon bulundu
        </span>
        <span class="text-muted small">{{ activeLabel }}</span>
      </div>
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th v-if="activeTab === 'customer'"><i class="bi bi-car-front me-1"></i>Araç</th>
              <th v-if="activeTab === 'vehicle'"><i class="bi bi-person me-1"></i>Müşteri</th>
              <th><i class="bi bi-calendar-event me-1"></i>Bitiş Tarihi</th>
              <th><i class="bi bi-cash-coin me-1"></i>Opsiyon Ücreti</th>
              <th><i class="bi bi-flag me-1"></i>Durum</th>
              <th><i class="bi bi-chat-left-text me-1"></i>Notlar</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="opt in options" :key="opt.id">
              <td v-if="activeTab === 'customer'">
                <strong>{{ opt.vehicleDisplayName }}</strong>
                <br>
                <code class="small text-primary">{{ opt.vehicleVIN }}</code>
              </td>
              <td v-if="activeTab === 'vehicle'">
                <strong>{{ opt.customerDisplayName }}</strong>
              </td>
              <td>
                <span :class="{ 'text-danger fw-semibold': opt.isExpired }">
                  {{ formatDate(opt.expiresAt) }}
                </span>
                <small v-if="opt.isExpired" class="d-block text-danger">Süresi doldu</small>
              </td>
              <td>
                <span v-if="opt.optionFeeAmount > 0">
                  {{ opt.optionFeeAmount.toLocaleString() }} {{ opt.optionFeeCurrency }}
                </span>
                <span v-else class="text-muted">Ücretsiz</span>
              </td>
              <td>
                <span :class="getOptionStatusClass(opt.status, opt.isExpired)" class="badge">
                  <i :class="getOptionStatusIcon(opt.status, opt.isExpired)" class="me-1"></i>
                  {{ getOptionStatusText(opt.status, opt.isExpired) }}
                </span>
              </td>
              <td>
                <span class="text-muted small">{{ opt.notes || '—' }}</span>
              </td>
              <td>
                <button
                  v-if="opt.status === 'Active' && !opt.isExpired"
                  class="btn btn-sm btn-outline-danger"
                  :disabled="cancellingId === opt.id"
                  @click="cancelOption(opt)"
                >
                  <span v-if="cancellingId === opt.id" class="spinner-border spinner-border-sm"></span>
                  <i v-else class="bi bi-x-circle"></i>
                  İptal
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="hasSearched" class="card shadow-sm border-0 text-center py-5">
      <div class="card-body">
        <i class="bi bi-bookmark-x display-1 text-muted mb-3"></i>
        <h5>Opsiyon bulunamadı</h5>
        <p class="text-muted">{{ activeLabel }} için kayıtlı opsiyon bulunmuyor.</p>
        <router-link to="/vehicle-options/new" class="btn btn-primary">
          <i class="bi bi-bookmark-plus-fill me-2"></i>Yeni Opsiyon Oluştur
        </router-link>
      </div>
    </div>

    <!-- Initial State -->
    <div v-else class="card shadow-sm border-0 text-center py-5 text-muted">
      <div class="card-body">
        <i class="bi bi-bookmark-star display-1 mb-3 opacity-25"></i>
        <p>{{ activeTab === 'vehicle' ? 'Araç seçerek opsiyon geçmişini görüntüleyin' : 'Müşteri seçerek opsiyonları görüntüleyin' }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { vehicleApi } from '../api/vehicle'
import { customerApi } from '../api/customer'
import { vehicleOptionApi } from '../api/vehicleOption'
import type { Vehicle } from '../types/vehicle'
import type { Customer, VehicleOption } from '../types/vehicleOption'

// ------ State ------
const activeTab = ref<'vehicle' | 'customer'>('vehicle')
const options = ref<VehicleOption[]>([])
const loading = ref(false)
const hasSearched = ref(false)
const cancellingId = ref<string | null>(null)
const alertMessage = ref('')
const alertType = ref<'success' | 'danger'>('success')

// Vehicle filter search
const vehicleSearch = ref('')
const vehicleResults = ref<Vehicle[]>([])
const vehicleSearchLoading = ref(false)
const showVehicleDropdown = ref(false)
const selectedVehicleId = ref<string | null>(null)
const selectedVehicleLabel = ref('')
let vehicleTimer: ReturnType<typeof setTimeout> | null = null

// Customer filter search
const customerSearch = ref('')
const customerResults = ref<Customer[]>([])
const customerSearchLoading = ref(false)
const showCustomerDropdown = ref(false)
const selectedCustomerId = ref<string | null>(null)
const selectedCustomerLabel = ref('')
let customerTimer: ReturnType<typeof setTimeout> | null = null

// Vehicle load
const vehicleLoading = ref(false)
const customerLoading = ref(false)

// ------ Computed ------
const activeLabel = computed(() => {
  if (activeTab.value === 'vehicle') return selectedVehicleLabel.value
  return selectedCustomerLabel.value
})

// ------ Tab Switch ------
const switchTab = (tab: 'vehicle' | 'customer') => {
  activeTab.value = tab
  options.value = []
  hasSearched.value = false
  alertMessage.value = ''
}

// ------ Vehicle search for filter ------
const onVehicleSearch = () => {
  selectedVehicleId.value = null
  showVehicleDropdown.value = false
  if (vehicleTimer) clearTimeout(vehicleTimer)
  if (!vehicleSearch.value.trim()) { vehicleResults.value = []; return }
  vehicleTimer = setTimeout(async () => {
    vehicleSearchLoading.value = true
    try {
      const result = await vehicleApi.getVehicles({ brand: vehicleSearch.value, pageSize: 20 })
      vehicleResults.value = result.items
      showVehicleDropdown.value = vehicleResults.value.length > 0
    } catch { vehicleResults.value = [] }
    finally { vehicleSearchLoading.value = false }
  }, 350)
}

const selectVehicleForFilter = (v: Vehicle) => {
  selectedVehicleId.value = v.id
  selectedVehicleLabel.value = `${v.brand} ${v.model} ${v.year}`
  vehicleSearch.value = `${v.brand} ${v.model} ${v.year}`
  showVehicleDropdown.value = false
  vehicleResults.value = []
}

const clearVehicleSearch = () => {
  vehicleSearch.value = ''
  selectedVehicleId.value = null
  vehicleResults.value = []
  showVehicleDropdown.value = false
  options.value = []
  hasSearched.value = false
}

const loadByVehicle = async () => {
  if (!selectedVehicleId.value) return
  vehicleLoading.value = true
  loading.value = true
  try {
    options.value = await vehicleOptionApi.getByVehicle(selectedVehicleId.value)
    hasSearched.value = true
  } catch (e: any) {
    showAlert(e.response?.data?.error || 'Opsiyonlar yüklenemedi', 'danger')
  } finally { loading.value = false; vehicleLoading.value = false }
}

// ------ Customer search for filter ------
const onCustomerSearch = () => {
  selectedCustomerId.value = null
  showCustomerDropdown.value = false
  if (customerTimer) clearTimeout(customerTimer)
  if (!customerSearch.value.trim()) { customerResults.value = []; return }
  customerTimer = setTimeout(async () => {
    customerSearchLoading.value = true
    try {
      const result = await customerApi.getCustomers({ search: customerSearch.value, pageSize: 20 })
      customerResults.value = result.items
      showCustomerDropdown.value = customerResults.value.length > 0
    } catch { customerResults.value = [] }
    finally { customerSearchLoading.value = false }
  }, 350)
}

const selectCustomerForFilter = (c: Customer) => {
  selectedCustomerId.value = c.id
  selectedCustomerLabel.value = `${c.firstName} ${c.lastName}`
  customerSearch.value = `${c.firstName} ${c.lastName}`
  showCustomerDropdown.value = false
  customerResults.value = []
}

const clearCustomerSearch = () => {
  customerSearch.value = ''
  selectedCustomerId.value = null
  customerResults.value = []
  showCustomerDropdown.value = false
  options.value = []
  hasSearched.value = false
}

const loadByCustomer = async () => {
  if (!selectedCustomerId.value) return
  customerLoading.value = true
  loading.value = true
  try {
    options.value = await vehicleOptionApi.getByCustomer(selectedCustomerId.value)
    hasSearched.value = true
  } catch (e: any) {
    showAlert(e.response?.data?.error || 'Opsiyonlar yüklenemedi', 'danger')
  } finally { loading.value = false; customerLoading.value = false }
}

// ------ Cancel option ------
const cancelOption = async (opt: VehicleOption) => {
  if (!confirm(`"${opt.vehicleDisplayName}" aracına ait opsiyon iptal edilsin mi?`)) return

  cancellingId.value = opt.id
  try {
    await vehicleOptionApi.cancelOption(opt.id)
    // Refresh list
    if (activeTab.value === 'vehicle' && selectedVehicleId.value) {
      options.value = await vehicleOptionApi.getByVehicle(selectedVehicleId.value)
    } else if (activeTab.value === 'customer' && selectedCustomerId.value) {
      options.value = await vehicleOptionApi.getByCustomer(selectedCustomerId.value)
    }
    showAlert('Opsiyon başarıyla iptal edildi. Araç statüsü "Satışta" olarak güncellendi.', 'success')
  } catch (e: any) {
    showAlert(e.response?.data?.error || 'Opsiyon iptal edilemedi', 'danger')
  } finally { cancellingId.value = null }
}

// ------ Helpers ------
const showAlert = (msg: string, type: 'success' | 'danger') => {
  alertMessage.value = msg
  alertType.value = type
  setTimeout(() => { alertMessage.value = '' }, 5000)
}

const formatDate = (iso: string) => {
  return new Date(iso).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' })
}

const getStatusClass = (status: string) => {
  const map: Record<string, string> = { InStock: 'bg-primary', OnSale: 'bg-success', Sold: 'bg-secondary', Reserved: 'bg-warning text-dark' }
  return map[status] || 'bg-secondary'
}

const getStatusText = (status: string) => {
  const map: Record<string, string> = { InStock: 'Stokta', OnSale: 'Satışta', Sold: 'Satıldı', Reserved: 'Rezerve' }
  return map[status] || status
}

const getOptionStatusClass = (status: string, isExpired: boolean) => {
  if (isExpired || status === 'Expired') return 'bg-warning text-dark'
  if (status === 'Active') return 'bg-success'
  if (status === 'Cancelled') return 'bg-secondary'
  return 'bg-secondary'
}

const getOptionStatusIcon = (status: string, isExpired: boolean) => {
  if (isExpired || status === 'Expired') return 'bi-hourglass-bottom'
  if (status === 'Active') return 'bi-bookmark-check'
  if (status === 'Cancelled') return 'bi-bookmark-x'
  return 'bi-circle'
}

const getOptionStatusText = (status: string, isExpired: boolean) => {
  if (isExpired || status === 'Expired') return 'Süresi Doldu'
  if (status === 'Active') return 'Aktif'
  if (status === 'Cancelled') return 'İptal Edildi'
  return status
}
</script>
