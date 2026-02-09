<template>
  <div>
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-1">
          <i class="bi bi-list-ul me-2 text-primary"></i>
          Araç Listesi
        </h2>
        <p class="text-muted">Envanterdeki tüm araçları görüntüleyin ve yönetin</p>
      </div>
    </div>

    <!-- Filters Card -->
    <div class="card shadow-sm border-0 mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label fw-semibold">
              <i class="bi bi-search me-1"></i>Marka
            </label>
            <input v-model="filters.brand" type="text" class="form-control" 
              placeholder="Marka ara..." @input="loadVehicles" />
          </div>
          <div class="col-md-3">
            <label class="form-label fw-semibold">
              <i class="bi bi-filter me-1"></i>Durum
            </label>
            <select v-model="filters.status" class="form-select" @change="loadVehicles">
              <option value="">Tüm Durumlar</option>
              <option value="InStock">Stokta</option>
              <option value="OnSale">Satışta</option>
              <option value="Sold">Satıldı</option>
              <option value="Reserved">Rezerve</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label fw-semibold">
              <i class="bi bi-grid-3x3-gap me-1"></i>Sayfa Boyutu
            </label>
            <select v-model="filters.pageSize" class="form-select" @change="loadVehicles">
              <option :value="5">5</option>
              <option :value="10">10</option>
              <option :value="25">25</option>
              <option :value="50">50</option>
            </select>
          </div>
          <div class="col-md-3 d-flex align-items-end">
            <router-link to="/vehicles/new" class="btn btn-success w-100">
              <i class="bi bi-plus-circle-fill me-2"></i>
              Yeni Araç Ekle
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status">
        <span class="visually-hidden">Yükleniyor...</span>
      </div>
      <p class="mt-3 text-muted">Araçlar yükleniyor...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="alert alert-danger shadow-sm" role="alert">
      <i class="bi bi-exclamation-triangle-fill me-2"></i>
      <strong>Hata:</strong> {{ error }}
    </div>

    <!-- Vehicle Table -->
    <div v-else-if="vehicles && vehicles.items.length > 0" class="card shadow-sm">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th><i class="bi bi-hash me-1"></i>VIN</th>
              <th><i class="bi bi-car-front me-1"></i>Araç</th>
              <th><i class="bi bi-calendar me-1"></i>Yıl</th>
              <th><i class="bi bi-speedometer me-1"></i>KM</th>
              <th><i class="bi bi-cash-coin me-1"></i>Fiyat</th>
              <th><i class="bi bi-flag-fill me-1"></i>Durum</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="vehicle in vehicles.items" :key="vehicle.id">
              <td>
                <code class="text-primary small fw-bold">{{ vehicle.vin }}</code>
              </td>
              <td>
                <strong>{{ vehicle.brand }}</strong> {{ vehicle.model }}
                <br>
                <small class="text-muted">
                  <i class="bi bi-palette me-1"></i>{{ vehicle.color }}
                </small>
              </td>
              <td>{{ vehicle.year }}</td>
              <td>{{ vehicle.mileage.toLocaleString() }} km</td>
              <td>
                <strong>{{ vehicle.suggestedAmount.toLocaleString() }}</strong>
                <small class="text-muted">{{ vehicle.suggestedCurrency }}</small>
              </td>
              <td>
                <span :class="getStatusClass(vehicle.status)" class="badge">
                  <i :class="getStatusIcon(vehicle.status)" class="me-1"></i>
                  {{ getStatusText(vehicle.status) }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="card-footer bg-light">
        <div class="row align-items-center">
          <div class="col-md-6">
            <span class="text-muted">
              <i class="bi bi-info-circle me-1"></i>
              Toplam <strong>{{ vehicles.totalCount }}</strong> araç bulundu
              (Sayfa {{ filters.page }} / {{ vehicles.totalPages }})
            </span>
          </div>
          <div class="col-md-6">
            <nav>
              <ul class="pagination justify-content-end mb-0">
                <li class="page-item" :class="{ disabled: filters.page === 1 }">
                  <button class="page-link" @click="previousPage" :disabled="filters.page === 1">
                    <i class="bi bi-chevron-left"></i> Önceki
                  </button>
                </li>
                <li class="page-item active">
                  <span class="page-link">{{ filters.page }}</span>
                </li>
                <li class="page-item" :class="{ disabled: filters.page >= vehicles.totalPages }">
                  <button class="page-link" @click="nextPage" 
                    :disabled="filters.page >= vehicles.totalPages">
                    Sonraki <i class="bi bi-chevron-right"></i>
                  </button>
                </li>
              </ul>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="card shadow-sm text-center py-5">
      <div class="card-body">
        <i class="bi bi-inbox display-1 text-muted mb-3"></i>
        <h4>Henüz araç bulunmuyor</h4>
        <p class="text-muted mb-4">Sisteme ilk aracı ekleyerek başlayın</p>
        <router-link to="/vehicles/new" class="btn btn-primary">
          <i class="bi bi-plus-circle-fill me-2"></i>
          Yeni Araç Ekle
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { vehicleApi } from '../api/vehicle'
import type { PagedResult, Vehicle } from '../types/vehicle'

const vehicles = ref<PagedResult<Vehicle> | null>(null)
const loading = ref(false)
const error = ref('')

const filters = reactive({
  page: 1,
  pageSize: 10,
  brand: '',
  status: ''
})

const loadVehicles = async () => {
  loading.value = true
  error.value = ''
  try {
    vehicles.value = await vehicleApi.getVehicles(filters)
  } catch (e: any) {
    error.value = e.response?.data?.error || e.message || 'Araçlar yüklenirken bir hata oluştu'
    console.error('API Error:', e)
  } finally {
    loading.value = false
  }
}

const nextPage = () => {
  if (vehicles.value && filters.page < vehicles.value.totalPages) {
    filters.page++
    loadVehicles()
  }
}

const previousPage = () => {
  if (filters.page > 1) {
    filters.page--
    loadVehicles()
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    InStock: 'bg-primary',
    OnSale: 'bg-success',
    Sold: 'bg-secondary',
    Reserved: 'bg-warning text-dark'
  }
  return classes[status] || 'bg-secondary'
}

const getStatusIcon = (status: string) => {
  const icons: Record<string, string> = {
    InStock: 'bi-box-seam',
    OnSale: 'bi-cart-check',
    Sold: 'bi-check-circle-fill',
    Reserved: 'bi-hourglass-split'
  }
  return icons[status] || 'bi-circle'
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    InStock: 'Stokta',
    OnSale: 'Satışta',
    Sold: 'Satıldı',
    Reserved: 'Rezerve'
  }
  return texts[status] || status
}

onMounted(() => {
  loadVehicles()
})
</script>
