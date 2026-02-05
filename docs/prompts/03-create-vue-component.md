# Prompt: Vue Component Geliştirme

## Amaç
User story'deki UI mockup'a göre Vue 3 Composition API ile component geliştirme.

## Prompt Template

```markdown
# UI Mockup
[docs/ui/ altındaki HTML mockup dosyasını referans et]

# User Story
[docs/business/US-XXX-xxx.md dosyasını referans et]

# Gereksinimler
- Vue 3 Composition API kullan
- TypeScript ile type-safe kod yaz
- Composables ile logic'i ayır
- Atomic Design prensiplerine uy
- Responsive tasarım
- Form validation (vuelidate veya yup)
- Loading states
- Error handling
- Accessibility (ARIA labels, keyboard navigation)

# Component Yapısı
Aşağıdaki component'leri oluştur:

1. Page Component: `pages/vehicles/add.vue`
2. Form Component: `components/organisms/VehicleForm.vue`
3. Composable: `composables/useVehicleForm.ts`
4. Types: `types/vehicle.ts`
5. API Service: `services/vehicleApi.ts` (gerekirse)

# Kodlama Standartları
- docs/architectural-overview/02-coding-standards.md dosyasındaki TypeScript/Vue standartlarına uy
- Props ve emits için TypeScript interface kullan
- Computed properties ile reactive state yönet
- Template'de v-if/v-show kullanımına dikkat et

# Örnek İstek

US-001: Araç Ekleme için Vue component'leri geliştir.

UI Mockup: docs/ui/vehicle-add-form.html

İstenen:
1. VehicleAddPage (pages/vehicles/add.vue)
2. VehicleForm component (organisms)
3. useVehicleForm composable (form logic, validation, submit)
4. Vehicle types (TypeScript interfaces)

Backend API:
- POST /api/v1/vehicles
- Request/Response DTO'ları user story'de tanımlı

Lütfen tüm dosyaları üret ve kodlama standartlarına uy.
```

## Kullanım

### Copilot Chat'te
```markdown
@workspace Vue component'leri oluştur.
Context: #file:docs/ui/vehicle-add-form.html
         #file:docs/business/US-001-add-vehicle-to-inventory.md
         #file:docs/architectural-overview/02-coding-standards.md
```

## Beklenen Çıktı Örneği

### types/vehicle.ts
```typescript
export interface Vehicle {
  id: string;
  vin: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  engineType: EngineType;
  transmissionType: TransmissionType;
  mileage: number;
  purchasePrice: Money;
  suggestedPrice: Money;
  status: VehicleStatus;
  features?: string[];
}

export interface CreateVehicleRequest {
  vin: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  engineType: EngineType;
  transmissionType: TransmissionType;
  purchaseAmount: number;
  purchaseCurrency: Currency;
  suggestedAmount: number;
  suggestedCurrency: Currency;
  engineCapacity?: number;
  fuelConsumption?: number;
  mileage: number;
  features?: string[];
}

export enum EngineType {
  Petrol = 'Petrol',
  Diesel = 'Diesel',
  Electric = 'Electric',
  Hybrid = 'Hybrid'
}

export enum TransmissionType {
  Manual = 'Manual',
  Automatic = 'Automatic',
  SemiAutomatic = 'SemiAutomatic'
}

export enum VehicleStatus {
  Available = 'Available',
  Reserved = 'Reserved',
  Sold = 'Sold'
}

export enum Currency {
  TRY = 'TRY',
  USD = 'USD',
  EUR = 'EUR'
}

export interface Money {
  amount: number;
  currency: Currency;
}
```

### composables/useVehicleForm.ts
```typescript
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import type { CreateVehicleRequest } from '@/types/vehicle'
import { vehicleApi } from '@/services/vehicleApi'
import { useNotification } from '@/composables/useNotification'

export function useVehicleForm() {
  const router = useRouter()
  const { showSuccess, showError } = useNotification()
  
  const loading = ref(false)
  const formData = ref<CreateVehicleRequest>({
    vin: '',
    brand: '',
    model: '',
    year: new Date().getFullYear(),
    color: '',
    engineType: 'Petrol',
    transmissionType: 'Manual',
    purchaseAmount: 0,
    purchaseCurrency: 'TRY',
    suggestedAmount: 0,
    suggestedCurrency: 'TRY',
    mileage: 0,
    features: []
  })
  
  const errors = ref<Record<string, string>>({})
  
  // Validation
  const validateVIN = (vin: string): boolean => {
    if (vin.length !== 17) {
      errors.value.vin = 'VIN 17 karakter olmalıdır'
      return false
    }
    if (/[IOQ]/.test(vin.toUpperCase())) {
      errors.value.vin = 'VIN I, O, Q karakterlerini içeremez'
      return false
    }
    delete errors.value.vin
    return true
  }
  
  const validateForm = (): boolean => {
    errors.value = {}
    
    if (!validateVIN(formData.value.vin)) return false
    
    if (!formData.value.brand) {
      errors.value.brand = 'Marka seçilmelidir'
      return false
    }
    
    if (!formData.value.model) {
      errors.value.model = 'Model seçilmelidir'
      return false
    }
    
    if (formData.value.purchaseAmount <= 0) {
      errors.value.purchaseAmount = 'Alış fiyatı pozitif olmalıdır'
      return false
    }
    
    if (formData.value.suggestedAmount < formData.value.purchaseAmount) {
      errors.value.suggestedAmount = 'Satış fiyatı alış fiyatından düşük olamaz'
      return false
    }
    
    return Object.keys(errors.value).length === 0
  }
  
  // Auto-calculate suggested price (15% markup)
  const calculateSuggestedPrice = () => {
    if (formData.value.purchaseAmount > 0) {
      formData.value.suggestedAmount = Math.round(
        formData.value.purchaseAmount * 1.15
      )
      formData.value.suggestedCurrency = formData.value.purchaseCurrency
    }
  }
  
  // Submit
  const submitForm = async () => {
    if (!validateForm()) {
      showError('Lütfen formu eksiksiz doldurun')
      return
    }
    
    loading.value = true
    
    try {
      const response = await vehicleApi.create(formData.value)
      showSuccess('Araç başarıyla eklendi')
      router.push(`/vehicles/${response.data}`)
    } catch (error: any) {
      if (error.response?.data?.message) {
        showError(error.response.data.message)
      } else {
        showError('Araç eklenirken bir hata oluştu')
      }
      console.error('Error adding vehicle:', error)
    } finally {
      loading.value = false
    }
  }
  
  // Reset form
  const resetForm = () => {
    formData.value = {
      vin: '',
      brand: '',
      model: '',
      year: new Date().getFullYear(),
      color: '',
      engineType: 'Petrol',
      transmissionType: 'Manual',
      purchaseAmount: 0,
      purchaseCurrency: 'TRY',
      suggestedAmount: 0,
      suggestedCurrency: 'TRY',
      mileage: 0,
      features: []
    }
    errors.value = {}
  }
  
  return {
    formData,
    errors,
    loading,
    validateVIN,
    validateForm,
    calculateSuggestedPrice,
    submitForm,
    resetForm
  }
}
```

### components/organisms/VehicleForm.vue
```vue
<script setup lang="ts">
import { watch } from 'vue'
import { useVehicleForm } from '@/composables/useVehicleForm'
import { BRANDS, COLORS, ENGINE_TYPES, TRANSMISSION_TYPES, VEHICLE_FEATURES } from '@/utils/constants'

const emit = defineEmits<{
  submit: []
  cancel: []
}>()

const {
  formData,
  errors,
  loading,
  validateVIN,
  calculateSuggestedPrice,
  submitForm
} = useVehicleForm()

// Watch purchase price changes to auto-calculate suggested price
watch(() => formData.value.purchaseAmount, () => {
  calculateSuggestedPrice()
})

const handleSubmit = async () => {
  await submitForm()
  emit('submit')
}

const handleCancel = () => {
  emit('cancel')
}
</script>

<template>
  <form @submit.prevent="handleSubmit" class="vehicle-form">
    <!-- Temel Bilgiler -->
    <section class="form-section">
      <h2>Temel Bilgiler</h2>
      
      <div class="form-grid">
        <div class="form-group">
          <label for="vin">
            VIN Numarası <span class="required">*</span>
          </label>
          <input
            id="vin"
            v-model="formData.vin"
            type="text"
            maxlength="17"
            placeholder="1HGBH41JXMN109186"
            required
            @blur="validateVIN(formData.vin)"
            :class="{ 'error': errors.vin }"
          />
          <span v-if="errors.vin" class="error-text">{{ errors.vin }}</span>
          <span v-else class="help-text">17 karakterli araç kimlik numarası</span>
        </div>
        
        <div class="form-group">
          <label for="brand">Marka <span class="required">*</span></label>
          <select
            id="brand"
            v-model="formData.brand"
            required
            :class="{ 'error': errors.brand }"
          >
            <option value="">Seçiniz...</option>
            <option v-for="brand in BRANDS" :key="brand.value" :value="brand.value">
              {{ brand.label }}
            </option>
          </select>
          <span v-if="errors.brand" class="error-text">{{ errors.brand }}</span>
        </div>
        
        <!-- Diğer form alanları... -->
      </div>
    </section>
    
    <!-- Fiyat Bilgileri -->
    <section class="form-section">
      <h2>Fiyat Bilgileri</h2>
      
      <div class="form-grid">
        <div class="form-group">
          <label for="purchaseAmount">Alış Fiyatı <span class="required">*</span></label>
          <div class="price-input">
            <input
              id="purchaseAmount"
              v-model.number="formData.purchaseAmount"
              type="number"
              min="0"
              step="1000"
              required
              :class="{ 'error': errors.purchaseAmount }"
            />
            <select v-model="formData.purchaseCurrency">
              <option value="TRY">TRY</option>
              <option value="USD">USD</option>
              <option value="EUR">EUR</option>
            </select>
          </div>
          <span v-if="errors.purchaseAmount" class="error-text">
            {{ errors.purchaseAmount }}
          </span>
        </div>
        
        <!-- Diğer fiyat alanları... -->
      </div>
    </section>
    
    <!-- Buttons -->
    <div class="button-group">
      <button
        type="button"
        class="btn-secondary"
        @click="handleCancel"
        :disabled="loading"
      >
        İptal
      </button>
      <button
        type="submit"
        class="btn-primary"
        :disabled="loading"
      >
        <span v-if="loading">Kaydediliyor...</span>
        <span v-else>Kaydet</span>
      </button>
    </div>
  </form>
</template>

<style scoped>
/* Component-specific styles */
.vehicle-form {
  max-width: 1000px;
  margin: 0 auto;
}

.form-section {
  margin-bottom: 32px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.form-group input.error,
.form-group select.error {
  border-color: #d32f2f;
}

.error-text {
  color: #d32f2f;
  font-size: 12px;
  margin-top: 4px;
}

/* ... diğer styles */
</style>
```

### pages/vehicles/add.vue
```vue
<script setup lang="ts">
import { useRouter } from 'vue-router'
import VehicleForm from '@/components/organisms/VehicleForm.vue'

const router = useRouter()

const handleSubmit = () => {
  // Form submission handled in composable
  // Just navigate or show success message
}

const handleCancel = () => {
  router.push('/vehicles')
}
</script>

<template>
  <div class="page-container">
    <header class="page-header">
      <h1>Yeni Araç Ekle</h1>
    </header>
    
    <main class="page-content">
      <VehicleForm
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </main>
  </div>
</template>

<style scoped>
.page-container {
  padding: 24px;
}

.page-header {
  margin-bottom: 24px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 600;
}
</style>
```

## Notlar
- Component'leri atomic design prensiplerine göre organize et
- Reusable logic'i composable'lara taşı
- Form validation library kullanımı opsiyonel (vuelidate, yup)
- Accessibility önemli (ARIA labels, keyboard navigation)
- Loading ve error states ekle
- Mobile responsive tasarım yap
