<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-2">Yeni Araç Ekle</h1>
      <p class="text-gray-600">Envantere yeni araç kaydı oluşturun</p>
    </div>

    <div v-if="successMessage" class="bg-green-50 border border-green-200 rounded-lg p-4 mb-6">
      <p class="text-green-800">✓ {{ successMessage }}</p>
    </div>

    <div v-if="errorMessage" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
      <p class="text-red-800">✗ {{ errorMessage }}</p>
    </div>

    <form @submit.prevent="submitForm" class="bg-white rounded-lg shadow-md p-6 space-y-6">
      <div class="grid md:grid-cols-2 gap-6">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">VIN (17 karakter)*</label>
          <input v-model="form.vin" type="text" maxlength="17" required
            class="w-full px-4 py-2 border rounded-lg" placeholder="1HGBH41JXMN109186" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Marka*</label>
          <input v-model="form.brand" type="text" required class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Model*</label>
          <input v-model="form.model" type="text" required class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Yıl*</label>
          <input v-model.number="form.year" type="number" :max="currentYear" required
            class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Motor Tipi*</label>
          <select v-model="form.engineType" required class="w-full px-4 py-2 border rounded-lg">
            <option value="">Seçiniz</option>
            <option value="Gasoline">Benzin</option>
            <option value="Diesel">Dizel</option>
            <option value="Electric">Elektrik</option>
            <option value="Hybrid">Hibrit</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Vites Tipi*</label>
          <select v-model="form.transmissionType" required class="w-full px-4 py-2 border rounded-lg">
            <option value="">Seçiniz</option>
            <option value="Manual">Manuel</option>
            <option value="Automatic">Otomatik</option>
            <option value="SemiAutomatic">Yarı Otomatik</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Kilometre*</label>
          <input v-model.number="form.mileage" type="number" min="0" required
            class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Renk*</label>
          <input v-model="form.color" type="text" required class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Alış Fiyatı*</label>
          <div class="flex gap-2">
            <input v-model.number="form.purchaseAmount" type="number" min="0" required
              class="flex-1 px-4 py-2 border rounded-lg" />
            <select v-model="form.purchaseCurrency" class="px-4 py-2 border rounded-lg">
              <option value="TRY">TRY</option>
              <option value="USD">USD</option>
              <option value="EUR">EUR</option>
            </select>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Satış Fiyatı*</label>
          <div class="flex gap-2">
            <input v-model.number="form.suggestedAmount" type="number" min="0" required
              class="flex-1 px-4 py-2 border rounded-lg" />
            <select v-model="form.suggestedCurrency" class="px-4 py-2 border rounded-lg">
              <option value="TRY">TRY</option>
              <option value="USD">USD</option>
              <option value="EUR">EUR</option>
            </select>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Yakıt Tüketimi (L/100km)*</label>
          <input v-model.number="form.fuelConsumption" type="number" step="0.1" min="0" required
            class="w-full px-4 py-2 border rounded-lg" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Motor Hacmi (cc)*</label>
          <input v-model.number="form.engineCapacity" type="number" min="0" required
            class="w-full px-4 py-2 border rounded-lg" />
        </div>
      </div>

      <div class="flex gap-4 justify-end">
        <NuxtLink to="/vehicles"
          class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50">
          İptal
        </NuxtLink>
        <button type="submit" :disabled="submitting"
          class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
          {{ submitting ? 'Kaydediliyor...' : 'Kaydet' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import type { CreateVehicleRequest } from '~/types/vehicle';

useHead({ title: 'Yeni Araç Ekle - Araç Envanter Yönetimi' });

const { createVehicle } = useVehicleApi();
const router = useRouter();

const currentYear = new Date().getFullYear();

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
});

const submitting = ref(false);
const successMessage = ref('');
const errorMessage = ref('');

const submitForm = async () => {
  submitting.value = true;
  successMessage.value = '';
  errorMessage.value = '';

  try {
    await createVehicle(form);
    successMessage.value = 'Araç başarıyla eklendi!';
    setTimeout(() => {
      router.push('/vehicles');
    }, 1500);
  } catch (e: any) {
    errorMessage.value = e.data?.error || e.message || 'Bir hata oluştu';
  } finally {
    submitting.value = false;
  }
};
</script>
