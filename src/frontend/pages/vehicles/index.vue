<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-2">Araç Listesi</h1>
      <p class="text-gray-600">Envanterdeki tüm araçları görüntüleyin ve yönetin</p>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow-md p-4 mb-6">
      <div class="grid md:grid-cols-3 gap-4">
        <input 
          v-model="filters.brand"
          type="text" 
          placeholder="Marka ara..." 
          class="px-4 py-2 border rounded-lg"
          @input="loadVehicles"
        />
        <select 
          v-model="filters.status"
          class="px-4 py-2 border rounded-lg"
          @change="loadVehicles"
        >
          <option value="">Tüm Durumlar</option>
          <option value="InStock">Stokta</option>
          <option value="OnSale">Satışta</option>
          <option value="Sold">Satıldı</option>
        </select>
        <NuxtLink 
          to="/vehicles/new"
          class="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 text-center"
        >
          + Yeni Araç Ekle
        </NuxtLink>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="pending" class="text-center py-12">
      <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      <p class="mt-4 text-gray-600">Yükleniyor...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800">Hata: {{ error }}</p>
    </div>

    <!-- Vehicle List -->
    <div v-else-if="vehicles" class="bg-white rounded-lg shadow-md overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">VIN</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Araç</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Yıl</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fiyat</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Durum</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="vehicle in vehicles.items" :key="vehicle.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 text-sm font-mono text-gray-900">{{ vehicle.vin }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ vehicle.brand }} {{ vehicle.model }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ vehicle.year }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">
              {{ vehicle.suggestedAmount.toLocaleString() }} {{ vehicle.suggestedCurrency }}
            </td>
            <td class="px-6 py-4 text-sm">
              <span :class="getStatusClass(vehicle.status)" class="px-2 py-1 rounded-full text-xs">
                {{ getStatusText(vehicle.status) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div class="bg-gray-50 px-6 py-4 flex items-center justify-between border-t">
        <div class="text-sm text-gray-700">Toplam {{ vehicles.totalCount }} araç</div>
        <div class="flex gap-2">
          <button 
            @click="previousPage"
            :disabled="filters.page === 1"
            class="px-4 py-2 border rounded-lg disabled:opacity-50"
          >
            Önceki
          </button>
          <span class="px-4 py-2">Sayfa {{ filters.page }} / {{ vehicles.totalPages }}</span>
          <button 
            @click="nextPage"
            :disabled="filters.page >= vehicles.totalPages"
            class="px-4 py-2 border rounded-lg disabled:opacity-50"
          >
            Sonraki
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { PagedResult, Vehicle, VehicleFilters } from '~/types/vehicle';

useHead({ title: 'Araç Listesi - Araç Envanter Yönetimi' });

const { getVehicles } = useVehicleApi();

const filters = reactive<VehicleFilters>({
  page: 1,
  pageSize: 10,
  brand: '',
  status: ''
});

const vehicles = ref<PagedResult<Vehicle> | null>(null);
const pending = ref(false);
const error = ref('');

const loadVehicles = async () => {
  pending.value = true;
  error.value = '';
  try {
    vehicles.value = await getVehicles(filters);
  } catch (e: any) {
    error.value = e.message || 'Araçlar yüklenirken bir hata oluştu';
  } finally {
    pending.value = false;
  }
};

const nextPage = () => {
  if (vehicles.value && filters.page! < vehicles.value.totalPages) {
    filters.page!++;
    loadVehicles();
  }
};

const previousPage = () => {
  if (filters.page! > 1) {
    filters.page!--;
    loadVehicles();
  }
};

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    InStock: 'bg-blue-100 text-blue-800',
    OnSale: 'bg-green-100 text-green-800',
    Sold: 'bg-gray-100 text-gray-800',
    Reserved: 'bg-yellow-100 text-yellow-800'
  };
  return classes[status] || 'bg-gray-100 text-gray-800';
};

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    InStock: 'Stokta',
    OnSale: 'Satışta',
    Sold: 'Satıldı',
    Reserved: 'Rezerve'
  };
  return texts[status] || status;
};

onMounted(() => {
  loadVehicles();
});
</script>
