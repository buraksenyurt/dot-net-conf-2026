<template>
  <div>
    <!-- Hoş Geldiniz Başlığı -->
    <div class="row align-items-center mb-4">
      <div class="col">
        <h2 class="mb-1 fw-bold">
          <i class="bi bi-person-badge me-2 text-primary"></i>
          Hoş geldiniz, {{ advisor?.firstName }} {{ advisor?.lastName }}
        </h2>
        <p class="text-muted mb-0">
          <span class="badge bg-primary bg-opacity-10 text-primary border border-primary border-opacity-25 me-2">
            <i class="bi bi-building me-1"></i>{{ advisor?.department }}
          </span>
          Kişisel opsiyon takip panelinize hoş geldiniz
        </p>
      </div>
      <div class="col-auto d-flex gap-2">
        <button class="btn btn-outline-secondary btn-sm" @click="loadData">
          <i class="bi bi-arrow-clockwise me-1"></i>Yenile
        </button>
        <button class="btn btn-outline-danger btn-sm" @click="handleLogout">
          <i class="bi bi-box-arrow-right me-1"></i>Çıkış
        </button>
      </div>
    </div>

    <!-- İstatistik Kartları -->
    <div class="row g-3 mb-4">
      <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body d-flex align-items-center gap-3">
            <div class="rounded-3 p-3 bg-primary bg-opacity-10">
              <i class="bi bi-bookmark-star-fill text-primary fs-4"></i>
            </div>
            <div>
              <div class="text-muted small">Toplam Opsiyon</div>
              <div class="fs-3 fw-bold lh-1">{{ stats.total }}</div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body d-flex align-items-center gap-3">
            <div class="rounded-3 p-3 bg-success bg-opacity-10">
              <i class="bi bi-check-circle-fill text-success fs-4"></i>
            </div>
            <div>
              <div class="text-muted small">Aktif</div>
              <div class="fs-3 fw-bold lh-1 text-success">{{ stats.active }}</div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body d-flex align-items-center gap-3">
            <div class="rounded-3 p-3 bg-warning bg-opacity-10">
              <i class="bi bi-clock-history text-warning fs-4"></i>
            </div>
            <div>
              <div class="text-muted small">Süresi Dolmuş</div>
              <div class="fs-3 fw-bold lh-1 text-warning">{{ stats.expired }}</div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body d-flex align-items-center gap-3">
            <div class="rounded-3 p-3 bg-secondary bg-opacity-10">
              <i class="bi bi-x-circle-fill text-secondary fs-4"></i>
            </div>
            <div>
              <div class="text-muted small">İptal Edilmiş</div>
              <div class="fs-3 fw-bold lh-1 text-secondary">{{ stats.cancelled }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filtre Sekmeler + Liste -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-white border-bottom d-flex align-items-center gap-2">
        <i class="bi bi-list-ul text-primary"></i>
        <span class="fw-semibold">Opsiyonlarım</span>
        <div class="ms-auto d-flex gap-1">
          <button
            v-for="f in filters"
            :key="f.key"
            class="btn btn-sm"
            :class="activeFilter === f.key ? 'btn-primary' : 'btn-outline-secondary'"
            @click="activeFilter = f.key"
          >
            {{ f.label }}
            <span class="badge rounded-pill ms-1"
              :class="activeFilter === f.key ? 'bg-white text-primary' : 'bg-secondary text-white'">
              {{ f.key === 'all' ? stats.total : options.filter(o => resolveStatus(o) === f.key).length }}
            </span>
          </button>
        </div>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="card-body text-center py-5">
        <span class="spinner-border text-primary"></span>
        <div class="text-muted mt-2 small">Opsiyonlar yükleniyor...</div>
      </div>

      <!-- Error -->
      <div v-else-if="error" class="card-body">
        <div class="alert alert-danger mb-0">
          <i class="bi bi-exclamation-triangle-fill me-2"></i>{{ error }}
        </div>
      </div>

      <!-- Boş veri -->
      <div v-else-if="filteredOptions.length === 0" class="card-body text-center py-5">
        <i class="bi bi-bookmark-x display-4 text-muted opacity-50 d-block mb-3"></i>
        <p class="text-muted mb-0">
          {{ activeFilter === 'all' ? 'Henüz hiç opsiyonunuz bulunmuyor.' : 'Bu filtreyle eşleşen opsiyon yok.' }}
        </p>
        <router-link to="/vehicle-options/new" class="btn btn-primary btn-sm mt-3">
          <i class="bi bi-bookmark-plus me-1"></i>Yeni Opsiyon Oluştur
        </router-link>
      </div>

      <!-- Opsiyon Kartları -->
      <div v-else class="card-body p-3">
        <div class="row g-3">
          <div v-for="opt in filteredOptions" :key="opt.id" class="col-md-6 col-xl-4">
            <div class="card h-100 border" :class="getCardBorder(opt)">
              <!-- Kart Başlığı -->
              <div class="card-header d-flex align-items-center gap-2 py-2"
                :class="getCardHeaderClass(opt)">
                <i class="bi bi-car-front-fill"></i>
                <span class="fw-semibold text-truncate flex-grow-1" style="font-size:0.9rem">
                  {{ opt.vehicleDisplayName }}
                </span>
                <span class="badge" :class="getStatusBadge(opt)">
                  {{ getStatusLabel(opt) }}
                </span>
              </div>

              <div class="card-body py-3 px-3">
                <!-- VIN -->
                <div class="mb-2">
                  <small class="text-muted d-block">VIN</small>
                  <code class="text-dark" style="font-size:0.78rem">{{ opt.vehicleVIN }}</code>
                </div>

                <!-- Müşteri -->
                <div class="mb-2">
                  <small class="text-muted d-block">Müşteri</small>
                  <div class="fw-semibold small d-flex align-items-center gap-1">
                    <i class="bi bi-person text-primary"></i>
                    {{ opt.customerDisplayName }}
                  </div>
                </div>

                <!-- Ücreti + Bitiş -->
                <div class="row g-2 mt-1">
                  <div class="col-6">
                    <div class="bg-light rounded-2 p-2 text-center">
                      <div class="text-muted" style="font-size:0.7rem">Opsiyon Ücreti</div>
                      <div class="fw-bold small">
                        {{ opt.optionFeeAmount > 0
                          ? opt.optionFeeAmount.toLocaleString('tr-TR') + ' ' + opt.optionFeeCurrency
                          : '—' }}
                      </div>
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="bg-light rounded-2 p-2 text-center">
                      <div class="text-muted" style="font-size:0.7rem">Bitiş Tarihi</div>
                      <div class="fw-bold small" :class="{ 'text-danger': opt.isExpired }">
                        {{ formatDate(opt.expiresAt) }}
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Kalan Süre -->
                <div v-if="resolveStatus(opt) === 'active'" class="mt-2">
                  <div class="d-flex justify-content-between align-items-center mb-1">
                    <small class="text-muted">Kalan Süre</small>
                    <small class="fw-semibold" :class="daysRemaining(opt) <= 3 ? 'text-danger' : 'text-success'">
                      {{ daysRemaining(opt) }} gün
                    </small>
                  </div>
                  <div class="progress" style="height:4px">
                    <div
                      class="progress-bar"
                      :class="daysRemaining(opt) <= 3 ? 'bg-danger' : 'bg-success'"
                      :style="{ width: progressWidth(opt) + '%' }"
                    ></div>
                  </div>
                </div>

                <!-- Notlar -->
                <div v-if="opt.notes" class="mt-2 text-muted small fst-italic text-truncate" :title="opt.notes">
                  <i class="bi bi-chat-left-text me-1"></i>{{ opt.notes }}
                </div>
              </div>

              <div class="card-footer bg-transparent border-top py-2 px-3">
                <small class="text-muted">
                  <i class="bi bi-calendar2 me-1"></i>
                  Oluşturuldu: {{ formatDate(opt.createdAt) }}
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { serviceAdvisorApi } from '../api/serviceAdvisor'
import type { VehicleOption } from '../types/vehicleOption'

const router = useRouter()
const { advisor, logout } = useAuth()

const options     = ref<VehicleOption[]>([])
const loading     = ref(false)
const error       = ref('')
const activeFilter = ref<'all' | 'active' | 'expired' | 'cancelled'>('all')

const filters = [
  { key: 'all',       label: 'Tümü' },
  { key: 'active',    label: 'Aktif' },
  { key: 'expired',   label: 'Süresi Dolmuş' },
  { key: 'cancelled', label: 'İptal Edilmiş' },
] as const

// ---- Stats ----
const stats = computed(() => ({
  total:     options.value.length,
  active:    options.value.filter(o => resolveStatus(o) === 'active').length,
  expired:   options.value.filter(o => resolveStatus(o) === 'expired').length,
  cancelled: options.value.filter(o => resolveStatus(o) === 'cancelled').length,
}))

// ---- Filtered ----
const filteredOptions = computed(() =>
  activeFilter.value === 'all'
    ? options.value
    : options.value.filter(o => resolveStatus(o) === activeFilter.value)
)

// ---- Helpers ----
function resolveStatus(o: VehicleOption): 'active' | 'expired' | 'cancelled' {
  if (o.status === 'Cancelled') return 'cancelled'
  if (o.isExpired || o.status === 'Expired') return 'expired'
  return 'active'
}

function getStatusLabel(o: VehicleOption): string {
  const s = resolveStatus(o)
  return s === 'active' ? 'Aktif' : s === 'expired' ? 'Süresi Doldu' : 'İptal'
}

function getStatusBadge(o: VehicleOption): string {
  return resolveStatus(o) === 'active'
    ? 'bg-success'
    : resolveStatus(o) === 'expired'
      ? 'bg-warning text-dark'
      : 'bg-secondary'
}

function getCardBorder(o: VehicleOption): string {
  return resolveStatus(o) === 'active'
    ? 'border-success border-opacity-50'
    : resolveStatus(o) === 'expired'
      ? 'border-warning border-opacity-50'
      : ''
}

function getCardHeaderClass(o: VehicleOption): string {
  return resolveStatus(o) === 'active'
    ? 'bg-success bg-opacity-10 text-success-emphasis'
    : resolveStatus(o) === 'expired'
      ? 'bg-warning bg-opacity-10 text-warning-emphasis'
      : 'bg-secondary bg-opacity-10 text-secondary'
}

function daysRemaining(o: VehicleOption): number {
  const diff = new Date(o.expiresAt).getTime() - Date.now()
  return Math.max(0, Math.ceil(diff / 86_400_000))
}

function progressWidth(o: VehicleOption): number {
  const total = (new Date(o.expiresAt).getTime() - new Date(o.createdAt).getTime()) / 86_400_000
  const remaining = daysRemaining(o)
  return total > 0 ? Math.min(100, (remaining / total) * 100) : 0
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('tr-TR', { day: '2-digit', month: 'short', year: 'numeric' })
}

// ---- Data ----
async function loadData() {
  if (!advisor.value) return
  loading.value = true
  error.value   = ''
  try {
    options.value = await serviceAdvisorApi.getDashboard(advisor.value.id)
  } catch (e: any) {
    error.value = e.response?.data?.error || 'Opsiyonlar yüklenirken hata oluştu'
  } finally {
    loading.value = false
  }
}

function handleLogout() {
  logout()
  router.push('/login')
}

onMounted(loadData)
</script>
