<template>
  <div>
    <!-- Stats Cards -->
    <div class="row g-4 mb-4">
      <div class="col-md-4">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body p-4 d-flex align-items-center">
            <div class="bg-primary bg-opacity-10 p-3 rounded-circle me-3">
              <i class="bi bi-car-front-fill text-primary fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted mb-1">Toplam Araç</h6>
              <h3 class="fw-bold mb-0">{{ totalVehicles }}</h3>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-md-4">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body p-4 d-flex align-items-center">
            <div class="bg-success bg-opacity-10 p-3 rounded-circle me-3">
              <i class="bi bi-currency-dollar text-success fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted mb-1">Toplam Değer</h6>
              <h3 class="fw-bold mb-0">₺{{ totalValue }}</h3>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body p-4 d-flex align-items-center">
             <div class="bg-warning bg-opacity-10 p-3 rounded-circle me-3">
              <i class="bi bi-lightning-charge-fill text-warning fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted mb-1">Son Aktivite</h6>
              <h5 class="fw-bold mb-0">Yeni Giriş</h5>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Info Section -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-white py-3">
        <h5 class="card-title mb-0">Hoş Geldiniz</h5>
      </div>
      <div class="card-body p-4">
        <p class="lead text-muted">
          Araç Envanter Yönetim Sistemi'ne hoş geldiniz. Sol menüyü kullanarak araç listesini görüntüleyebilir veya yeni araç girişi yapabilirsiniz.
        </p>
        <hr>
        <div class="row mt-4">
          <div class="col-md-6">
             <h6><i class="bi bi-check-circle-fill text-success me-2"></i>Sistem Durumu</h6>
             <p class="text-muted small">Tüm servisler aktif ve çalışır durumda.</p>
          </div>
           <div class="col-md-6">
             <h6><i class="bi bi-clock-fill text-primary me-2"></i>Son Güncelleme</h6>
             <p class="text-muted small">{{ new Date().toLocaleDateString('tr-TR') }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { vehicleApi } from '../api/vehicle'

const totalVehicles = ref(0)
const totalValue = ref('0')

const loadStats = async () => {
  try {
    const data = await vehicleApi.getVehicles({ pageSize: 1 })
    totalVehicles.value = data.totalCount
    // Mock value calculation or implement aggregation endpoint
    totalValue.value = "7.450.000" 
  } catch (e) {
    console.error('Stats loading error', e)
  }
}

onMounted(() => {
  loadStats()
})
</script>
