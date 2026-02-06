<template>
  <div>
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-1">
          <i class="bi bi-plus-circle-dotted me-2 text-primary"></i>
          Yeni Araç Ekle
        </h2>
        <p class="text-muted">Envantere yeni bir araç kaydı oluşturun</p>
      </div>
    </div>

    <div v-if="successMessage" class="alert alert-success d-flex align-items-center mb-4" role="alert">
      <i class="bi bi-check-circle-fill flex-shrink-0 me-2"></i>
      <div>{{ successMessage }}</div>
    </div>

    <div v-if="errorMessage" class="alert alert-danger d-flex align-items-center mb-4" role="alert">
      <i class="bi bi-exclamation-triangle-fill flex-shrink-0 me-2"></i>
      <div>{{ errorMessage }}</div>
    </div>

    <div class="card shadow-sm border-0">
      <div class="card-header bg-white py-3">
        <h5 class="card-title mb-0">Araç Bilgileri</h5>
      </div>
      <div class="card-body p-4">
        <form @submit.prevent="submitForm" class="row g-3">
          <!-- VIN -->
          <div class="col-12">
            <label class="form-label fw-semibold">VIN (Şasi No)*</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-upc-scan"></i></span>
              <input v-model="form.vin" type="text" maxlength="17" required
                class="form-control" placeholder="17 haneli şasi numarası" />
            </div>
            <div class="form-text">Benzersiz 17 karakterli araç kimlik numarası</div>
          </div>

              <!-- Brand & Model -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Marka*</label>
                <input v-model="form.brand" type="text" required class="form-control" placeholder="Örn: Tesla" />
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Model*</label>
                <input v-model="form.model" type="text" required class="form-control" placeholder="Örn: Model Y" />
              </div>

              <!-- Year & Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Yıl*</label>
                <input v-model.number="form.year" type="number" :max="currentYear" required class="form-control" />
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Renk*</label>
                <input v-model="form.color" type="text" required class="form-control" placeholder="Örn: Beyaz" />
              </div>

              <!-- Technical Specs -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Motor Tipi*</label>
                <select v-model="form.engineType" required class="form-select">
                  <option value="">Seçiniz</option>
                  <option value="Gasoline">Benzin</option>
                  <option value="Diesel">Dizel</option>
                  <option value="Electric">Elektrik</option>
                  <option value="Hybrid">Hibrit</option>
                </select>
              </div>
              <div class="col-md-6">
                <label class="form-label fw-semibold">Vites Tipi*</label>
                <select v-model="form.transmissionType" required class="form-select">
                  <option value="">Seçiniz</option>
                  <option value="Manual">Manuel</option>
                  <option value="Automatic">Otomatik</option>
                  <option value="SemiAutomatic">Yarı Otomatik</option>
                </select>
              </div>

              <!-- Mileage & Engine Info -->
              <div class="col-md-4">
                <label class="form-label fw-semibold">Kilometre*</label>
                <div class="input-group">
                  <input v-model.number="form.mileage" type="number" min="0" required class="form-control" />
                  <span class="input-group-text">km</span>
                </div>
              </div>
              <div class="col-md-4">
                <label class="form-label fw-semibold">Yakıt Tüketimi*</label>
                <div class="input-group">
                  <input v-model.number="form.fuelConsumption" type="number" step="0.1" min="0" required class="form-control" />
                  <span class="input-group-text">L/100km</span>
                </div>
              </div>
              <div class="col-md-4">
                <label class="form-label fw-semibold">Motor Hacmi*</label>
                <div class="input-group">
                  <input v-model.number="form.engineCapacity" type="number" min="0" required class="form-control" />
                  <span class="input-group-text">cc</span>
                </div>
              </div>

              <!-- Pricing -->
              <div class="col-12"><hr class="my-2"></div>
              
              <div class="col-md-6">
                <label class="form-label fw-semibold text-secondary">Alış Fiyatı*</label>
                <div class="input-group">
                  <input v-model.number="form.purchaseAmount" type="number" min="0" required class="form-control" />
                  <select v-model="form.purchaseCurrency" class="form-select" style="max-width: 100px;">
                    <option value="TRY">TRY</option>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                  </select>
                </div>
              </div>

              <div class="col-md-6">
                <label class="form-label fw-semibold text-success">Satış Fiyatı*</label>
                <div class="input-group">
                  <input v-model.number="form.suggestedAmount" type="number" min="0" required class="form-control" />
                  <select v-model="form.suggestedCurrency" class="form-select" style="max-width: 100px;">
                    <option value="TRY">TRY</option>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                  </select>
                </div>
              </div>

              <!-- Actions -->
              <div class="col-12 mt-4 d-flex justify-content-end gap-2">
                <router-link to="/vehicles" class="btn btn-outline-secondary px-4">
                  İptal
                </router-link>
                <button type="submit" :disabled="submitting" class="btn btn-primary px-4">
                  <span v-if="submitting" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  {{ submitting ? 'Kaydediliyor...' : 'Kaydet' }}
                </button>
              </div>
            </form>
          </div>
        </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { vehicleApi } from '../api/vehicle'
import type { CreateVehicleRequest } from '../types/vehicle'

const router = useRouter()
const currentYear = new Date().getFullYear()

const form = reactive<CreateVehicleRequest>({
  vin: '',
  brand: '',
  model: '',
  year: currentYear,
  engineType: '',
  mileage: 0,
  color: '',
  purchaseAmount: 0,
  purchaseCurrency: 'TRY',
  suggestedAmount: 0,
  suggestedCurrency: 'TRY',
  transmissionType: '',
  fuelConsumption: 0,
  engineCapacity: 0,
  features: []
})

const submitting = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const submitForm = async () => {
  submitting.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    await vehicleApi.createVehicle(form)
    successMessage.value = 'Araç başarıyla eklendi!'
    setTimeout(() => {
      router.push('/vehicles')
    }, 1500)
  } catch (e: any) {
    errorMessage.value = e.response?.data?.error || e.message || 'Bir hata oluştu'
  } finally {
    submitting.value = false
  }
}
</script>
