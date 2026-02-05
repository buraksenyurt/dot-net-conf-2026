# Kodlama Standartları

## Genel Prensipler

### SOLID Prensipleri
- **Single Responsibility**: Her sınıf tek bir sorumluluğa sahip olmalı
- **Open/Closed**: Genişlemeye açık, değişime kapalı
- **Liskov Substitution**: Alt sınıflar üst sınıfların yerine kullanılabilmeli
- **Interface Segregation**: Büyük interface'ler yerine spesifik interface'ler
- **Dependency Inversion**: Somut sınıflara değil, soyutlamalara bağımlı olun

### Clean Code
- Anlamlı ve açıklayıcı isimlendirme
- Fonksiyonlar kısa ve odaklı olmalı (max 20-30 satır)
- Yorum satırları yerine açıklayıcı kod
- Magic number'lar yerine named constant'lar
- Kod tekrarından kaçının (DRY - Don't Repeat Yourself)

## C# Kodlama Standartları

### Naming Conventions

```csharp
// Class, Interface, Method, Property: PascalCase
public class VehicleInventory
public interface IVehicleRepository
public void AddVehicle()
public string VehicleIdentificationNumber { get; set; }

// Private field: _camelCase
private readonly IVehicleRepository _vehicleRepository;

// Local variable, parameter: camelCase
public void ProcessVehicle(string vinNumber)
{
    var vehicle = GetVehicle(vinNumber);
}

// Constant: PascalCase veya UPPER_CASE
public const int MaxVehicleCount = 1000;
private const string DEFAULT_STATUS = "Available";
```

### Dosya ve Klasör Yapısı

```
src/
├── VehicleManagement.Api/           # Web API projesi
├── VehicleManagement.Application/   # Use case'ler ve CQRS
│   ├── Commands/
│   ├── Queries/
│   └── Validators/
├── VehicleManagement.Domain/        # Domain modeller ve iş kuralları
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Aggregates/
│   └── Exceptions/
├── VehicleManagement.Infrastructure/ # Dış bağımlılıklar
│   ├── Persistence/
│   ├── ExternalServices/
│   └── Messaging/
└── VehicleManagement.Tests/         # Test projeleri
```

### Exception Handling

```csharp
// Domain exception'lar kullanın
public class VehicleNotFoundException : DomainException
{
    public VehicleNotFoundException(string vin) 
        : base($"Araç bulunamadı. VIN: {vin}")
    {
    }
}

// Global exception handler ile yakalayın
// Loglayın ama stack trace'i kullanıcıya göstermeyin
```

### Async/Await Kullanımı

```csharp
// Tüm I/O operasyonları async olmalı
public async Task<Vehicle> GetVehicleAsync(string vin, CancellationToken cancellationToken)
{
    return await _repository.FindByVinAsync(vin, cancellationToken);
}

// Suffix olarak "Async" kullanın
```

### Dependency Injection

```csharp
// Constructor injection tercih edin
public class VehicleService
{
    private readonly IVehicleRepository _repository;
    private readonly ILogger<VehicleService> _logger;
    
    public VehicleService(
        IVehicleRepository repository, 
        ILogger<VehicleService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}

// Interface'leri her zaman soyutlama için kullanın
```

## TypeScript/Vue Kodlama Standartları

### Naming Conventions

```typescript
// Component: PascalCase
VehicleList.vue
VehicleDetailCard.vue

// Composables: camelCase with "use" prefix
useVehicleInventory.ts
useVehicleFilters.ts

// Types/Interfaces: PascalCase
interface Vehicle {
  id: string;
  vin: string;
  model: string;
}

// Enum: PascalCase
enum VehicleStatus {
  Available = 'available',
  Reserved = 'reserved',
  Sold = 'sold'
}
```

### Component Yapısı

```vue
<script setup lang="ts">
// 1. Imports
import { ref, computed, onMounted } from 'vue'
import { useVehicleInventory } from '@/composables/useVehicleInventory'

// 2. Props ve Emits
interface Props {
  vehicleId: string
}

const props = defineProps<Props>()
const emit = defineEmits<{
  vehicleSelected: [vehicle: Vehicle]
}>()

// 3. Composables
const { vehicles, loading, fetchVehicles } = useVehicleInventory()

// 4. Reactive state
const selectedVehicle = ref<Vehicle | null>(null)

// 5. Computed properties
const availableVehicles = computed(() => 
  vehicles.value.filter(v => v.status === 'available')
)

// 6. Methods
const handleSelect = (vehicle: Vehicle) => {
  selectedVehicle.value = vehicle
  emit('vehicleSelected', vehicle)
}

// 7. Lifecycle hooks
onMounted(async () => {
  await fetchVehicles()
})
</script>

<template>
  <!-- Template içeriği -->
</template>

<style scoped>
/* Component-specific styles */
</style>
```

### Composables Pattern

```typescript
// Yeniden kullanılabilir mantık için composables kullanın
export function useVehicleInventory() {
  const vehicles = ref<Vehicle[]>([])
  const loading = ref(false)
  const error = ref<Error | null>(null)

  const fetchVehicles = async () => {
    loading.value = true
    try {
      const response = await api.get('/vehicles')
      vehicles.value = response.data
    } catch (e) {
      error.value = e as Error
    } finally {
      loading.value = false
    }
  }

  return {
    vehicles: readonly(vehicles),
    loading: readonly(loading),
    error: readonly(error),
    fetchVehicles
  }
}
```

## Commit Message Standartları

Conventional Commits formatını kullanın:

```
feat: yeni araç envanter listeleme özelliği eklendi
fix: VIN numarası validasyonu düzeltildi
docs: API dokümantasyonu güncellendi
test: araç ekleme unit testleri eklendi
refactor: repository katmanı yeniden yapılandırıldı
chore: dependency'ler güncellendi
```

## Code Review Kuralları

- Her PR en az bir reviewer tarafından onaylanmalı
- SonarQube quality gate'i geçmeli (coverage > 80%)
- Build başarılı olmalı
- Testler geçmeli
- Açıklayıcı PR description yazılmalı
- Breaking change'ler belirtilmeli

## Loglama Standartları

```csharp
// Structured logging kullanın
_logger.LogInformation("Araç eklendi. VIN: {Vin}, Model: {Model}", vehicle.Vin, vehicle.Model);

// Log seviyeleri:
// - Trace: Detaylı debug bilgisi
// - Debug: Geliştirme sırasında faydalı bilgi
// - Information: Genel bilgilendirme
// - Warning: Beklenmeyen durum ama hata değil
// - Error: İşlem başarısız oldu
// - Critical: Sistem kritik durumda
```
