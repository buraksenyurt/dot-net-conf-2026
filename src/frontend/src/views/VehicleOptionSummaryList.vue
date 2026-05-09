<script setup lang="ts">
import { onMounted } from 'vue'
import { VehicleOptionStatus } from '../types/vehicleOption'
import { useVehicleOptionSummary } from '../composables/useVehicleOptionSummary'

const {
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
} = useVehicleOptionSummary()

onMounted(() => load())
</script>

<template>
  <div>
    <!-- Page Header -->
    <div class="row mb-4">
      <div class="col">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-1">
            <li class="breadcrumb-item">
              <router-link to="/">Anasayfa</router-link>
            </li>
            <li class="breadcrumb-item active" aria-current="page">Araç Opsiyonlama Listesi</li>
          </ol>
        </nav>
        <h1 class="h3 mb-1">
          <i class="bi bi-bookmark-star me-2 text-primary" aria-hidden="true"></i>
          Araç Opsiyonlama Özet Listesi
        </h1>
        <p class="text-muted mb-0">Tüm danışmanlara ait opsiyonlama kayıtlarını filtreleyin ve sorgulayın.</p>
      </div>
    </div>

    <!-- ===================== FILTER FORM ===================== -->
    <div class="card shadow-sm border-0 mb-4">
      <div class="card-body">
        <form
          role="search"
          aria-label="Opsiyon filtreleri"
          novalidate
          @submit.prevent="applyFilters"
          @reset.prevent="resetFilters"
        >
          <div class="row g-3 align-items-end">

            <!-- Müşteri Arama -->
            <div class="col-12 col-md-6 col-lg-3">
              <label for="customer-search" class="form-label fw-semibold">
                <i class="bi bi-person me-1" aria-hidden="true"></i>Müşteri Adı / Soyadı
              </label>
              <input
                id="customer-search"
                v-model="query.customerSearch"
                type="search"
                class="form-control"
                placeholder="Örn: John Doe"
                aria-describedby="customer-search-hint"
              />
              <div id="customer-search-hint" class="form-text text-muted">
                Ad veya soyad ile kısmi arama
              </div>
            </div>

            <!-- Araç / VIN Arama -->
            <div class="col-12 col-md-6 col-lg-3">
              <label for="vehicle-search" class="form-label fw-semibold">
                <i class="bi bi-car-front me-1" aria-hidden="true"></i>Araç / VIN
              </label>
              <input
                id="vehicle-search"
                v-model="query.vehicleSearch"
                type="search"
                class="form-control"
                placeholder="Marka, model veya VIN"
                aria-describedby="vehicle-search-hint"
              />
              <div id="vehicle-search-hint" class="form-text text-muted">
                Marka, model veya VIN ile kısmi arama
              </div>
            </div>

            <!-- Durum Filtresi -->
            <div class="col-12 col-md-6 col-lg-2">
              <label for="status-filter" class="form-label fw-semibold">
                <i class="bi bi-funnel me-1" aria-hidden="true"></i>Durum
              </label>
              <select
                id="status-filter"
                v-model="query.status"
                class="form-select"
              >
                <option :value="null">Tümü</option>
                <option :value="VehicleOptionStatus.Active">Aktif</option>
                <option :value="VehicleOptionStatus.Expired">Süresi Dolmuş</option>
                <option :value="VehicleOptionStatus.Cancelled">İptal Edilmiş</option>
              </select>
            </div>

            <!-- Tarihten -->
            <div class="col-12 col-md-3 col-lg-2">
              <label for="created-from" class="form-label fw-semibold">
                <i class="bi bi-calendar me-1" aria-hidden="true"></i>Tarihten
              </label>
              <input
                id="created-from"
                v-model="query.createdFrom"
                type="date"
                class="form-control"
                aria-describedby="date-range-hint"
              />
              <div id="date-range-hint" class="form-text text-muted">
                Oluşturulma tarihi aralığı
              </div>
            </div>

            <!-- Tarihe -->
            <div class="col-12 col-md-3 col-lg-2">
              <label for="created-to" class="form-label fw-semibold">
                <i class="bi bi-calendar-check me-1" aria-hidden="true"></i>Tarihe
              </label>
              <input
                id="created-to"
                v-model="query.createdTo"
                type="date"
                class="form-control"
                aria-describedby="date-range-hint"
              />
            </div>

            <!-- Butonlar -->
            <div class="col-12 col-lg-auto d-flex gap-2 align-items-end">
              <button type="submit" class="btn btn-primary" :disabled="loading">
                <span
                  v-if="loading"
                  class="spinner-border spinner-border-sm me-1"
                  role="status"
                  aria-hidden="true"
                ></span>
                <i v-else class="bi bi-search me-1" aria-hidden="true"></i>
                Filtrele
              </button>
              <button type="reset" class="btn btn-outline-secondary" :disabled="loading">
                <i class="bi bi-x-circle me-1" aria-hidden="true"></i>Temizle
              </button>
            </div>

          </div>
        </form>
      </div>
    </div>
    <!-- END FILTER FORM -->

    <!-- ===================== ERROR STATE (AC-4) ===================== -->
    <div
      v-if="error"
      class="alert alert-danger d-flex align-items-center mb-4"
      role="alert"
      aria-live="assertive"
    >
      <i class="bi bi-exclamation-triangle-fill flex-shrink-0 me-2" aria-hidden="true"></i>
      <div>{{ error }}</div>
    </div>

    <!-- ===================== LOADING STATE ===================== -->
    <div v-if="loading && !result" class="text-center py-5">
      <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status">
        <span class="visually-hidden">Yükleniyor...</span>
      </div>
      <p class="mt-3 text-muted">Opsiyonlar yükleniyor...</p>
    </div>

    <!-- ===================== RESULTS TABLE ===================== -->
    <div v-else-if="result">

      <!-- Tablo Üstü: Kayıt sayısı + Sayfa boyutu seçici -->
      <div class="d-flex justify-content-between align-items-center mb-3">
        <small class="text-muted" role="status" aria-live="polite">
          Toplam <strong>{{ result.totalCount }}</strong> kayıt
          (Sayfa {{ result.page }} / {{ result.totalPages }})
        </small>
        <div class="d-flex align-items-center gap-2">
          <label for="page-size" class="form-label mb-0 text-nowrap small">Sayfa boyutu:</label>
          <select
            id="page-size"
            v-model.number="query.pageSize"
            class="form-select form-select-sm"
            style="width: auto;"
            aria-label="Sayfa başına kayıt sayısı"
            @change="applyFilters"
          >
            <option :value="10">10</option>
            <option :value="20">20</option>
            <option :value="50">50</option>
          </select>
        </div>
      </div>

      <!-- Tablo -->
      <div class="card shadow-sm border-0">
        <div class="table-responsive">
          <table
            class="table table-hover align-middle mb-0"
            aria-label="Araç opsiyonlama özet listesi"
          >
            <thead class="table-light">
              <tr>
                <th scope="col" :aria-sort="getAriaSort('customerName')">
                  <button
                    class="btn btn-link btn-sm p-0 text-dark text-decoration-none fw-semibold"
                    aria-label="Müşteri adına göre sırala"
                    @click="setSort('customerName')"
                  >
                    <i class="bi bi-person me-1" aria-hidden="true"></i>Müşteri
                    <i :class="['bi ms-1', getSortIcon('customerName')]" aria-hidden="true"></i>
                  </button>
                </th>
                <th scope="col" :aria-sort="getAriaSort('vehicleDisplayName')">
                  <button
                    class="btn btn-link btn-sm p-0 text-dark text-decoration-none fw-semibold"
                    aria-label="Araç adına göre sırala"
                    @click="setSort('vehicleDisplayName')"
                  >
                    <i class="bi bi-car-front me-1" aria-hidden="true"></i>Araç
                    <i :class="['bi ms-1', getSortIcon('vehicleDisplayName')]" aria-hidden="true"></i>
                  </button>
                </th>
                <th scope="col">VIN</th>
                <th scope="col">Durum</th>
                <th scope="col" :aria-sort="getAriaSort('expiresAt')">
                  <button
                    class="btn btn-link btn-sm p-0 text-dark text-decoration-none fw-semibold"
                    aria-label="Bitiş tarihine göre sırala"
                    @click="setSort('expiresAt')"
                  >
                    <i class="bi bi-calendar-event me-1" aria-hidden="true"></i>Bitiş Tarihi
                    <i :class="['bi ms-1', getSortIcon('expiresAt')]" aria-hidden="true"></i>
                  </button>
                </th>
                <th scope="col" class="text-end">
                  <i class="bi bi-cash-coin me-1" aria-hidden="true"></i>Opsiyon Ücreti
                </th>
                <th scope="col">
                  <i class="bi bi-person-badge me-1" aria-hidden="true"></i>Servis Danışmanı
                </th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="item in result.items"
                :key="item.id"
              >
                <td class="fw-medium">{{ item.customerDisplayName }}</td>
                <td>
                  <span class="fw-medium">{{ item.vehicleDisplayName }}</span>
                </td>
                <td>
                  <code class="text-body small">{{ item.vehicleVIN }}</code>
                </td>
                <td>
                  <span
                    :class="getStatusBadgeClass(item)"
                    :aria-label="`Durum: ${getStatusLabel(item)}`"
                  >
                    {{ getStatusLabel(item) }}
                  </span>
                </td>
                <td>
                  <time :datetime="item.expiresAt">{{ formatDate(item.expiresAt) }}</time>
                </td>
                <td class="text-end fw-medium">
                  {{ formatCurrency(item.optionFeeAmount, item.optionFeeCurrency) }}
                </td>
                <td class="text-muted small">{{ item.serviceAdvisorDisplayName }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination (AC-1, BR-4) -->
        <div class="card-footer bg-light">
          <div class="row align-items-center">
            <div class="col-md-6">
              <span class="text-muted small">
                <i class="bi bi-info-circle me-1" aria-hidden="true"></i>
                Toplam <strong>{{ result.totalCount }}</strong> kayıt —
                Sayfa <strong>{{ result.page }}</strong> / <strong>{{ result.totalPages }}</strong>
              </span>
            </div>
            <div class="col-md-6">
              <nav aria-label="Opsiyon listesi sayfalama">
                <ul class="pagination pagination-sm justify-content-end mb-0">
                  <li class="page-item" :class="{ disabled: result.page === 1 }">
                    <button
                      class="page-link"
                      :disabled="result.page === 1"
                      aria-label="Önceki sayfa"
                      @click="goToPage(result.page - 1)"
                    >
                      <i class="bi bi-chevron-left" aria-hidden="true"></i>
                    </button>
                  </li>

                  <li
                    v-for="p in result.totalPages"
                    :key="p"
                    class="page-item"
                    :class="{ active: p === result.page }"
                    :aria-current="p === result.page ? 'page' : undefined"
                  >
                    <button class="page-link" @click="goToPage(p)">{{ p }}</button>
                  </li>

                  <li class="page-item" :class="{ disabled: result.page >= result.totalPages }">
                    <button
                      class="page-link"
                      :disabled="result.page >= result.totalPages"
                      aria-label="Sonraki sayfa"
                      @click="goToPage(result.page + 1)"
                    >
                      <i class="bi bi-chevron-right" aria-hidden="true"></i>
                    </button>
                  </li>
                </ul>
              </nav>
            </div>
          </div>
        </div>
      </div>

      <!-- ===================== EMPTY STATE (AC-4) ===================== -->
      <div
        v-if="result.items.length === 0"
        class="text-center py-5 text-muted"
        role="status"
        aria-live="polite"
      >
        <i class="bi bi-inbox fs-1 d-block mb-3" aria-hidden="true"></i>
        <p class="fw-medium mb-1">Opsiyonlama kaydı bulunamadı.</p>
        <p class="small mb-3">Arama kriterlerinizi değiştirip tekrar deneyebilirsiniz.</p>
        <button type="button" class="btn btn-outline-secondary btn-sm" @click="resetFilters">
          <i class="bi bi-x-circle me-1" aria-hidden="true"></i>Filtreleri Temizle
        </button>
      </div>

    </div>
    <!-- END RESULTS -->

  </div>
</template>
