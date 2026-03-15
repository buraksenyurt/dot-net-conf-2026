<template>
  <div>
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-1">
          <i class="bi bi-bookmark-check me-2 text-primary"></i>
          Araç Opsiyonlama
        </h2>
        <p class="text-muted">Bir aracı müşteri adına satın alma opsiyonu ile rezerve edin</p>
      </div>
    </div>

    <!-- Alerts -->
    <div v-if="successMessage" class="alert alert-success d-flex align-items-center mb-4" role="alert">
      <i class="bi bi-check-circle-fill flex-shrink-0 me-2"></i>
      <div>{{ successMessage }}</div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger d-flex align-items-center mb-4" role="alert">
      <i class="bi bi-exclamation-triangle-fill flex-shrink-0 me-2"></i>
      <div>{{ errorMessage }}</div>
    </div>

    <div class="row g-4">
      <!-- Sol: Form -->
      <div class="col-lg-7">
        <div class="card shadow-sm border-0">
          <div class="card-header bg-white fw-semibold border-bottom">
            <i class="bi bi-pencil-square me-2 text-primary"></i>Opsiyon Bilgileri
          </div>
          <div class="card-body p-4">
            <form @submit.prevent="submitForm">

              <!-- Araç Arama -->
              <div class="mb-4">
                <label class="form-label fw-semibold">Araç <span class="text-danger">*</span></label>
                <div class="input-group mb-2">
                  <span class="input-group-text"><i class="bi bi-car-front"></i></span>
                  <input
                    v-model="vehicleSearch"
                    type="text"
                    class="form-control"
                    placeholder="Marka veya model ile ara..."
                    @input="onVehicleSearch"
                    @focus="showVehicleDropdown = vehicleResults.length > 0"
                  />
                  <button v-if="vehicleSearch" class="btn btn-outline-secondary" type="button" @click="clearVehicle">
                    <i class="bi bi-x"></i>
                  </button>
                </div>

                <!-- Araç Arama Sonuçları -->
                <div v-if="showVehicleDropdown && vehicleResults.length > 0" class="list-group border shadow-sm mb-2" style="max-height:200px;overflow-y:auto;">
                  <button
                    v-for="v in vehicleResults"
                    :key="v.id"
                    type="button"
                    class="list-group-item list-group-item-action py-2"
                    @click="selectVehicle(v)"
                  >
                    <div class="d-flex justify-content-between align-items-center">
                      <div>
                        <strong>{{ v.brand }} {{ v.model }} {{ v.year }}</strong>
                        <small class="d-block text-muted">
                          <code>{{ v.vin }}</code> · {{ v.mileage.toLocaleString() }} km
                        </small>
                      </div>
                      <span :class="getStatusClass(v.status)" class="badge ms-2">
                        {{ getStatusText(v.status) }}
                      </span>
                    </div>
                  </button>
                </div>
                <div v-if="vehicleSearchLoading" class="text-muted small">
                  <span class="spinner-border spinner-border-sm me-1"></span> Aranıyor...
                </div>
                <div v-if="selectedVehicle" class="alert alert-info py-2 mb-0 d-flex align-items-center gap-2">
                  <i class="bi bi-check-circle-fill text-success"></i>
                  <span>
                    <strong>{{ selectedVehicle.brand }} {{ selectedVehicle.model }} {{ selectedVehicle.year }}</strong>
                    — {{ selectedVehicle.suggestedAmount.toLocaleString() }} {{ selectedVehicle.suggestedCurrency }}
                  </span>
                </div>
              </div>

              <!-- Müşteri Arama -->
              <div class="mb-4">
                <label class="form-label fw-semibold">Müşteri <span class="text-danger">*</span></label>
                <div class="input-group mb-2">
                  <span class="input-group-text"><i class="bi bi-person"></i></span>
                  <input
                    v-model="customerSearch"
                    type="text"
                    class="form-control"
                    placeholder="Ad, soyad veya e-posta ile ara..."
                    @input="onCustomerSearch"
                    @focus="showCustomerDropdown = customerResults.length > 0"
                  />
                  <button v-if="customerSearch" class="btn btn-outline-secondary" type="button" @click="clearCustomer">
                    <i class="bi bi-x"></i>
                  </button>
                  <button class="btn btn-outline-success" type="button" @click="openNewCustomerModal" title="Yeni müşteri ekle">
                    <i class="bi bi-person-plus me-1"></i>Yeni Müşteri
                  </button>
                </div>

                <!-- Müşteri Arama Sonuçları -->
                <div v-if="showCustomerDropdown && customerResults.length > 0" class="list-group border shadow-sm mb-2" style="max-height:200px;overflow-y:auto;">
                  <button
                    v-for="c in customerResults"
                    :key="c.id"
                    type="button"
                    class="list-group-item list-group-item-action py-2"
                    @click="selectCustomer(c)"
                  >
                    <strong>{{ c.firstName }} {{ c.lastName }}</strong>
                    <small class="d-block text-muted">{{ c.email }} · {{ c.customerType === 'Corporate' ? 'Kurumsal' : 'Bireysel' }}</small>
                  </button>
                </div>
                <div v-if="customerSearchLoading" class="text-muted small">
                  <span class="spinner-border spinner-border-sm me-1"></span> Aranıyor...
                </div>
                <div v-if="selectedCustomer" class="alert alert-info py-2 mb-0 d-flex align-items-center gap-2">
                  <i class="bi bi-check-circle-fill text-success"></i>
                  <span>
                    <strong>{{ selectedCustomer.firstName }} {{ selectedCustomer.lastName }}</strong>
                    — {{ selectedCustomer.email }}
                  </span>
                </div>
              </div>

              <!-- Opsiyon Süresi -->
              <div class="mb-3">
                <label class="form-label fw-semibold">
                  Opsiyon Süresi <span class="text-danger">*</span>
                </label>
                <div class="input-group" style="max-width:200px">
                  <input
                    v-model.number="form.validityDays"
                    type="number"
                    min="1"
                    max="30"
                    required
                    class="form-control"
                  />
                  <span class="input-group-text">gün</span>
                </div>
                <div class="form-text">
                  <i class="bi bi-calendar-event me-1"></i>
                  Bitiş tarihi: <strong>{{ expiresAtFormatted }}</strong>
                  <span class="text-muted ms-1">(1–30 gün)</span>
                </div>
              </div>

              <!-- Opsiyon Ücreti -->
              <div class="mb-3">
                <label class="form-label fw-semibold">Opsiyon Ücreti (Depozito)</label>
                <div class="input-group" style="max-width:300px">
                  <span class="input-group-text"><i class="bi bi-cash-coin"></i></span>
                  <input
                    v-model.number="form.optionFeeAmount"
                    type="number"
                    min="0"
                    step="100"
                    class="form-control"
                    placeholder="0"
                  />
                  <select v-model="form.optionFeeCurrency" class="form-select" style="max-width:90px">
                    <option value="TRY">TRY</option>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                  </select>
                </div>
                <div class="form-text">Sıfır (0) değer geçerlidir — ücretsiz opsiyon</div>
              </div>

              <!-- Notlar -->
              <div class="mb-4">
                <label class="form-label fw-semibold">Notlar</label>
                <textarea v-model="form.notes" class="form-control" rows="3"
                  placeholder="Opsiyonla ilgili ek notlar..." maxlength="500"></textarea>
                <div class="form-text text-end">{{ (form.notes || '').length }} / 500</div>
              </div>

              <!-- Butonlar -->
              <div class="d-flex gap-2">
                <button
                  type="submit"
                  class="btn btn-primary px-4"
                  :disabled="submitting || !selectedVehicle || !selectedCustomer"
                >
                  <span v-if="submitting" class="spinner-border spinner-border-sm me-2"></span>
                  <i v-else class="bi bi-bookmark-check me-2"></i>
                  {{ submitting ? 'Kaydediliyor...' : 'Opsiyonla' }}
                </button>
                <router-link to="/vehicle-options" class="btn btn-outline-secondary px-4">
                  İptal
                </router-link>
              </div>
            </form>
          </div>
        </div>
      </div>

      <!-- Sağ: Özet Kartı -->
      <div class="col-lg-5">
        <div class="card shadow-sm border-0 mb-3">
          <div class="card-header bg-white fw-semibold border-bottom">
            <i class="bi bi-info-circle me-2 text-secondary"></i>Seçilen Araç
          </div>
          <div class="card-body">
            <template v-if="selectedVehicle">
              <dl class="row mb-0 small">
                <dt class="col-5 text-muted">Araç</dt>
                <dd class="col-7 fw-semibold">{{ selectedVehicle.brand }} {{ selectedVehicle.model }} {{ selectedVehicle.year }}</dd>
                <dt class="col-5 text-muted">VIN</dt>
                <dd class="col-7"><code>{{ selectedVehicle.vin }}</code></dd>
                <dt class="col-5 text-muted">Renk</dt>
                <dd class="col-7">{{ selectedVehicle.color }}</dd>
                <dt class="col-5 text-muted">KM</dt>
                <dd class="col-7">{{ selectedVehicle.mileage.toLocaleString() }} km</dd>
                <dt class="col-5 text-muted">Satış Fiyatı</dt>
                <dd class="col-7 fw-bold text-success">{{ selectedVehicle.suggestedAmount.toLocaleString() }} {{ selectedVehicle.suggestedCurrency }}</dd>
                <dt class="col-5 text-muted">Durum</dt>
                <dd class="col-7">
                  <span :class="getStatusClass(selectedVehicle.status)" class="badge">
                    {{ getStatusText(selectedVehicle.status) }}
                  </span>
                </dd>
              </dl>
            </template>
            <div v-else class="text-center text-muted py-3">
              <i class="bi bi-car-front display-6 d-block mb-2 opacity-25"></i>
              Araç seçilmedi
            </div>
          </div>
        </div>

        <div class="card shadow-sm border-0">
          <div class="card-header bg-white fw-semibold border-bottom">
            <i class="bi bi-person me-2 text-secondary"></i>Seçilen Müşteri
          </div>
          <div class="card-body">
            <template v-if="selectedCustomer">
              <dl class="row mb-0 small">
                <dt class="col-5 text-muted">Ad Soyad</dt>
                <dd class="col-7 fw-semibold">{{ selectedCustomer.firstName }} {{ selectedCustomer.lastName }}</dd>
                <dt class="col-5 text-muted">E-posta</dt>
                <dd class="col-7">{{ selectedCustomer.email }}</dd>
                <dt class="col-5 text-muted">Telefon</dt>
                <dd class="col-7">{{ selectedCustomer.phone }}</dd>
                <dt class="col-5 text-muted">Tip</dt>
                <dd class="col-7">
                  <span :class="selectedCustomer.customerType === 'Corporate' ? 'badge bg-info text-dark' : 'badge bg-secondary'">
                    {{ selectedCustomer.customerType === 'Corporate' ? 'Kurumsal' : 'Bireysel' }}
                  </span>
                </dd>
              </dl>
            </template>
            <div v-else class="text-center text-muted py-3">
              <i class="bi bi-person display-6 d-block mb-2 opacity-25"></i>
              Müşteri seçilmedi
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Yeni Müşteri Ekle Modal -->
    <div
      class="modal fade"
      id="newCustomerModal"
      ref="newCustomerModalRef"
      tabindex="-1"
      aria-labelledby="newCustomerModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="newCustomerModalLabel">
              <i class="bi bi-person-plus me-2 text-success"></i>Yeni Müşteri Ekle
            </h5>
            <button type="button" class="btn-close" @click="closeNewCustomerModal" aria-label="Kapat"></button>
          </div>
          <div class="modal-body">
            <div v-if="newCustomerError" class="alert alert-danger py-2 mb-3" role="alert">
              <i class="bi bi-exclamation-triangle-fill me-2"></i>{{ newCustomerError }}
            </div>
            <div class="mb-3">
              <label for="ncFirstName" class="form-label fw-semibold">Ad <span class="text-danger">*</span></label>
              <input
                id="ncFirstName"
                v-model="newCustomerForm.firstName"
                type="text"
                class="form-control"
                required
                maxlength="100"
                placeholder="Müşteri adı"
              />
            </div>
            <div class="mb-3">
              <label for="ncLastName" class="form-label fw-semibold">Soyad <span class="text-danger">*</span></label>
              <input
                id="ncLastName"
                v-model="newCustomerForm.lastName"
                type="text"
                class="form-control"
                required
                maxlength="100"
                placeholder="Müşteri soyadı"
              />
            </div>
            <div class="mb-3">
              <label for="ncEmail" class="form-label fw-semibold">E-posta <span class="text-danger">*</span></label>
              <input
                id="ncEmail"
                v-model="newCustomerForm.email"
                type="email"
                class="form-control"
                required
                placeholder="ornek@email.com"
              />
            </div>
            <div class="mb-3">
              <label for="ncPhone" class="form-label fw-semibold">Telefon <span class="text-danger">*</span></label>
              <input
                id="ncPhone"
                v-model="newCustomerForm.phone"
                type="tel"
                class="form-control"
                required
                placeholder="+90 5XX XXX XX XX"
              />
            </div>
            <div class="mb-3">
              <label class="form-label fw-semibold">Müşteri Tipi <span class="text-danger">*</span></label>
              <div class="d-flex gap-3">
                <div class="form-check">
                  <input
                    class="form-check-input"
                    type="radio"
                    v-model="newCustomerForm.customerType"
                    value="Individual"
                    id="ncTypeIndividual"
                  />
                  <label class="form-check-label" for="ncTypeIndividual">Bireysel</label>
                </div>
                <div class="form-check">
                  <input
                    class="form-check-input"
                    type="radio"
                    v-model="newCustomerForm.customerType"
                    value="Corporate"
                    id="ncTypeCorporate"
                  />
                  <label class="form-check-label" for="ncTypeCorporate">Kurumsal</label>
                </div>
              </div>
            </div>
            <div v-if="newCustomerForm.customerType === 'Corporate'" class="mb-3">
              <label for="ncCompanyName" class="form-label fw-semibold">Firma Adı <span class="text-danger">*</span></label>
              <input
                id="ncCompanyName"
                v-model="newCustomerForm.companyName"
                type="text"
                class="form-control"
                :required="newCustomerForm.customerType === 'Corporate'"
                maxlength="200"
                placeholder="Firma adı"
              />
            </div>
            <div v-if="newCustomerForm.customerType === 'Corporate'" class="mb-3">
              <label for="ncTaxNumber" class="form-label fw-semibold">Vergi No <span class="text-danger">*</span></label>
              <input
                id="ncTaxNumber"
                v-model="newCustomerForm.taxNumber"
                type="text"
                class="form-control"
                :required="newCustomerForm.customerType === 'Corporate'"
                maxlength="50"
                placeholder="Vergi numarası"
              />
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-outline-secondary" @click="closeNewCustomerModal">
              <i class="bi bi-x-circle me-1"></i>Vazgeç
            </button>
            <button
              type="button"
              class="btn btn-success"
              @click="submitNewCustomer"
              :disabled="newCustomerSubmitting"
            >
              <span v-if="newCustomerSubmitting" class="spinner-border spinner-border-sm me-2"></span>
              <i v-else class="bi bi-person-check me-1"></i>
              {{ newCustomerSubmitting ? 'Kaydediliyor...' : 'Müşteriyi Ekle' }}
            </button>
          </div>
        </div>
      </div>
    </div>
</div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Modal } from 'bootstrap'
import { useRouter } from 'vue-router'
import { vehicleApi } from '../api/vehicle'
import { customerApi } from '../api/customer'
import { vehicleOptionApi } from '../api/vehicleOption'
import { useAuth } from '../composables/useAuth'
import type { Vehicle } from '../types/vehicle'
import type { Customer, CreateVehicleOptionRequest, CreateCustomerRequest } from '../types/vehicleOption'

const router = useRouter()
const { advisor } = useAuth()

// ------ State ------
const submitting = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

// Vehicle search
const vehicleSearch = ref('')
const vehicleResults = ref<Vehicle[]>([])
const vehicleSearchLoading = ref(false)
const showVehicleDropdown = ref(false)
const selectedVehicle = ref<Vehicle | null>(null)
let vehicleSearchTimer: ReturnType<typeof setTimeout> | null = null

// Customer search
const customerSearch = ref('')
const customerResults = ref<Customer[]>([])
const customerSearchLoading = ref(false)
const showCustomerDropdown = ref(false)
const selectedCustomer = ref<Customer | null>(null)
let customerSearchTimer: ReturnType<typeof setTimeout> | null = null

// New customer modal
const newCustomerModalRef = ref<HTMLElement | null>(null)
let newCustomerModal: Modal | null = null
const newCustomerSubmitting = ref(false)
const newCustomerError = ref('')
const newCustomerForm = reactive<CreateCustomerRequest>({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  customerType: 'Individual',
  companyName: '',
  taxNumber: ''
})

// Form
const form = reactive<Omit<CreateVehicleOptionRequest, 'vehicleId' | 'customerId'>>({
  validityDays: 7,
  optionFeeAmount: 0,
  optionFeeCurrency: 'TRY',
  notes: ''
})

// ------ Computed ------
const expiresAtFormatted = computed(() => {
  const d = new Date()
  d.setDate(d.getDate() + (form.validityDays || 0))
  return d.toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' })
})

onMounted(() => {
  if (newCustomerModalRef.value) {
    newCustomerModal = new Modal(newCustomerModalRef.value)
  }
})

// ------ Vehicle search ------
const onVehicleSearch = () => {
  selectedVehicle.value = null
  showVehicleDropdown.value = false
  if (vehicleSearchTimer) clearTimeout(vehicleSearchTimer)
  if (!vehicleSearch.value.trim()) { vehicleResults.value = []; return }
  vehicleSearchTimer = setTimeout(async () => {
    vehicleSearchLoading.value = true
    try {
      const result = await vehicleApi.getVehicles({ brand: vehicleSearch.value, pageSize: 20 })
      // Filter to only optionable vehicles
      vehicleResults.value = result.items.filter(v => v.status === 'InStock' || v.status === 'OnSale')
      showVehicleDropdown.value = vehicleResults.value.length > 0
    } catch { vehicleResults.value = [] }
    finally { vehicleSearchLoading.value = false }
  }, 350)
}

const selectVehicle = (v: Vehicle) => {
  selectedVehicle.value = v
  vehicleSearch.value = `${v.brand} ${v.model} ${v.year}`
  showVehicleDropdown.value = false
  vehicleResults.value = []
}

const clearVehicle = () => {
  vehicleSearch.value = ''
  selectedVehicle.value = null
  vehicleResults.value = []
  showVehicleDropdown.value = false
}

// ------ Customer search ------
const onCustomerSearch = () => {
  selectedCustomer.value = null
  showCustomerDropdown.value = false
  if (customerSearchTimer) clearTimeout(customerSearchTimer)
  if (!customerSearch.value.trim()) { customerResults.value = []; return }
  customerSearchTimer = setTimeout(async () => {
    customerSearchLoading.value = true
    try {
      const result = await customerApi.getCustomers({ search: customerSearch.value, pageSize: 20 })
      customerResults.value = result.items
      showCustomerDropdown.value = customerResults.value.length > 0
    } catch { customerResults.value = [] }
    finally { customerSearchLoading.value = false }
  }, 350)
}

const selectCustomer = (c: Customer) => {
  selectedCustomer.value = c
  customerSearch.value = `${c.firstName} ${c.lastName}`
  showCustomerDropdown.value = false
  customerResults.value = []
}

const clearCustomer = () => {
  customerSearch.value = ''
  selectedCustomer.value = null
  customerResults.value = []
  showCustomerDropdown.value = false
}

// ------ New customer modal ------
const openNewCustomerModal = () => {
  Object.assign(newCustomerForm, {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    customerType: 'Individual',
    companyName: '',
    taxNumber: ''
  })
  newCustomerError.value = ''
  newCustomerModal?.show()
}

const closeNewCustomerModal = () => {
  newCustomerModal?.hide()
}

const submitNewCustomer = async () => {
  newCustomerSubmitting.value = true
  newCustomerError.value = ''

  // Validate Corporate-specific required fields
  if (newCustomerForm.customerType === 'Corporate') {
    if (!newCustomerForm.companyName?.trim()) {
      newCustomerError.value = 'Kurumsal müşteri için firma adı zorunludur'
      newCustomerSubmitting.value = false
      return
    }
    if (!newCustomerForm.taxNumber?.trim()) {
      newCustomerError.value = 'Kurumsal müşteri için vergi numarası zorunludur'
      newCustomerSubmitting.value = false
      return
    }
  }

  try {
    const isCorporate = newCustomerForm.customerType === 'Corporate'
    const request: CreateCustomerRequest = {
      firstName: newCustomerForm.firstName.trim(),
      lastName: newCustomerForm.lastName.trim(),
      email: newCustomerForm.email.trim(),
      phone: newCustomerForm.phone.trim(),
      customerType: newCustomerForm.customerType,
      companyName: isCorporate ? newCustomerForm.companyName?.trim() : undefined,
      taxNumber: isCorporate ? newCustomerForm.taxNumber?.trim() : undefined
    }
    const { id } = await customerApi.createCustomer(request)
    const newCustomer: Customer = {
      id,
      firstName: newCustomerForm.firstName.trim(),
      lastName: newCustomerForm.lastName.trim(),
      email: newCustomerForm.email.trim(),
      phone: newCustomerForm.phone.trim(),
      customerType: newCustomerForm.customerType,
      companyName: isCorporate ? newCustomerForm.companyName?.trim() : undefined,
      taxNumber: isCorporate ? newCustomerForm.taxNumber?.trim() : undefined,
      createdAt: new Date().toISOString()
    }
    selectCustomer(newCustomer)
    closeNewCustomerModal()
  } catch (e: any) {
    newCustomerError.value = e.response?.data?.error || e.message || 'Müşteri eklenirken hata oluştu'
  } finally {
    newCustomerSubmitting.value = false
  }
}

// ------ Submit ------
const submitForm = async () => {
  if (!selectedVehicle.value || !selectedCustomer.value) return

  submitting.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    await vehicleOptionApi.createOption({
      vehicleId: selectedVehicle.value.id,
      customerId: selectedCustomer.value.id,
      validityDays: form.validityDays,
      optionFeeAmount: form.optionFeeAmount,
      optionFeeCurrency: form.optionFeeCurrency,
      serviceAdvisorId: advisor.value?.id || undefined,
      notes: form.notes || undefined
    })
    successMessage.value = `Araç başarıyla opsiyonlandı! ${expiresAtFormatted.value} tarihine kadar rezerve edildi.`
    setTimeout(() => router.push('/vehicle-options'), 2000)
  } catch (e: any) {
    errorMessage.value = e.response?.data?.error || e.message || 'Opsiyon oluşturulurken hata oluştu'
  } finally {
    submitting.value = false
  }
}

// ------ Helpers ------
const getStatusClass = (status: string) => {
  const map: Record<string, string> = {
    InStock: 'bg-primary', OnSale: 'bg-success', Sold: 'bg-secondary', Reserved: 'bg-warning text-dark'
  }
  return map[status] || 'bg-secondary'
}

const getStatusText = (status: string) => {
  const map: Record<string, string> = {
    InStock: 'Stokta', OnSale: 'Satışta', Sold: 'Satıldı', Reserved: 'Rezerve'
  }
  return map[status] || status
}
</script>
