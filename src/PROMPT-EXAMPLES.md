# AI Prompt Örnekleri - Demo Uygulama

Bu dosya, demo uygulamanın kalan kısımlarını üretmek için kullanabileceğiniz hazır AI prompt örneklerini içerir.

---

## 📦 Prompt 1: Vehicle Entity Oluşturma

### Context Dosyaları
- `docs/domain-model/entity-vehicle.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `src/backend/VehicleInventory.Domain/ValueObjects/VIN.cs`
- `src/backend/VehicleInventory.Domain/ValueObjects/Money.cs`

### Prompt

```
Aşağıdaki dosyaları oku:
- docs/domain-model/entity-vehicle.md
- docs/business/US-001-add-vehicle-to-inventory.md

Vehicle aggregate root entity'sini oluştur:

Hedef dosya: src/backend/VehicleInventory.Domain/Entities/Vehicle.cs

Gereksinimler:
1. DDD Entity prensiplerine uy (private setter, factory method)
2. VIN ve Money value object'lerini kullan (zaten mevcut)
3. Vehicle properties:
   - Id (Guid)
   - VIN (value object)
   - Brand (string)
   - Model (string)
   - Year (int)
   - EngineType (enum - oluşturulacak)
   - Mileage (int)
   - Color (string)
   - PurchasePrice (Money)
   - SuggestedPrice (Money)
   - TransmissionType (enum - oluşturulacak)
   - FuelConsumption (decimal)
   - EngineCapacity (int)
   - Features (List<string>)
   - Status (enum - oluşturulacak)
   - CreatedAt (DateTime)

4. Gerekli enum'ları da oluştur:
   - src/backend/VehicleInventory.Domain/Enums/EngineType.cs (Gasoline, Diesel, Electric, Hybrid)
   - src/backend/VehicleInventory.Domain/Enums/TransmissionType.cs (Manual, Automatic, SemiAutomatic)
   - src/backend/VehicleInventory.Domain/Enums/VehicleStatus.cs (InStock, OnSale, Sold, Reserved)

5. Factory method Create() ekle (validasyonlarla)
6. Business rule: SuggestedPrice >= PurchasePrice
7. C# 13 features kullan (primary constructors, file-scoped namespace)
8. XML documentation comments ekle

İmplementasyonu yap.
```

---

## 📦 Prompt 2: Add Vehicle Command (CQRS)

### Context Dosyaları
- `docs/prompts/01-create-api-endpoint.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `src/backend/VehicleInventory.Domain/Entities/Vehicle.cs` (önceki adımda oluşturuldu)

### Prompt

```
CQRS pattern kullanarak AddVehicleCommand oluştur.

Hedef dosyalar:
- src/backend/VehicleInventory.Application/Commands/AddVehicleCommand.cs
- src/backend/VehicleInventory.Application/Commands/AddVehicleCommandHandler.cs
- src/backend/VehicleInventory.Application/Commands/AddVehicleCommandValidator.cs

Context:
- docs/business/US-001-add-vehicle-to-inventory.md (business rules)
- docs/prompts/01-create-api-endpoint.md (command pattern örneği)

AddVehicleCommand (record):
```csharp
public record AddVehicleCommand(
    string Vin,
    string Brand,
    string Model,
    int Year,
    string EngineType,
    int Mileage,
    string Color,
    decimal PurchaseAmount,
    string PurchaseCurrency,
    decimal SuggestedAmount,
    string SuggestedCurrency,
    string TransmissionType,
    decimal FuelConsumption,
    int EngineCapacity,
    List<string> Features
);
```

AddVehicleCommandHandler:
- IRequestHandler<AddVehicleCommand, Result<Guid>> implement et
- Vehicle.Create() factory method kullan
- Repository pattern ile kaydet (IVehicleRepository - interface tanımla)
- Domain events yayınla (VehicleAddedEvent)
- Result pattern ile dön

AddVehicleCommandValidator (FluentValidation):
- VIN 17 karakter zorunlu
- Year <= DateTime.Now.Year
- Mileage >= 0
- PurchaseAmount > 0
- SuggestedAmount >= PurchaseAmount
- Brand, Model, Color required

US-001 acceptance criteria'larına uy.
```

---

## 📦 Prompt 3: Vehicles API Controller

### Context Dosyaları
- `docs/prompts/01-create-api-endpoint.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `docs/business/US-002-list-vehicles.md`

### Prompt

```
ASP.NET Core Web API Controller oluştur:

Hedef dosya: src/backend/VehicleInventory.API/Controllers/VehiclesController.cs

Endpoints:
1. POST /api/v1/vehicles
   - AddVehicleCommand gönder
   - 201 Created dön (Location header ile)
   - Validation errors için 400 Bad Request
   - Duplicate VIN için 409 Conflict

2. GET /api/v1/vehicles
   - Query parameters: page, pageSize, brand, status
   - GetVehiclesQuery gönder
   - Paged response dön

3. GET /api/v1/vehicles/{id}
   - GetVehicleByIdQuery gönder
   - 200 OK veya 404 Not Found

Requirements:
- MediatR ile command/query dispatch
- [ApiController] attribute kullan
- Swagger/OpenAPI annotations ([ProducesResponseType])
- Async/await pattern
- Proper HTTP status codes
- Error handling (ProblemDetails)

Örnek POST response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "vin": "1HGBH41JXMN109186",
  "brand": "Honda",
  "message": "Vehicle added successfully"
}
```

docs/prompts/01-create-api-endpoint.md'deki pattern'i takip et.
```

---

## 📦 Prompt 4: Vehicle List Vue Component

### Context Dosyaları
- `docs/prompts/03-create-vue-component.md`
- `docs/business/US-002-list-vehicles.md`
- `docs/ui/vehicle-list.html`
- `src/frontend/pages/vehicles/index.vue` (placeholder)

### Prompt

```
Vehicle list sayfasını tamamla.

Hedef dosya: src/frontend/pages/vehicles/index.vue (mevcut placeholder'ı değiştir)

Context:
- docs/business/US-002-list-vehicles.md (user story)
- docs/ui/vehicle-list.html (UI mockup)
- docs/prompts/03-create-vue-component.md (Vue component pattern)

Özellikler:
1. **Data Fetching**
   - useFetch() veya $fetch ile GET /api/v1/vehicles
   - Pagination support (page, pageSize)
   - Loading state
   - Error handling

2. **UI Components**
   - Responsive table (Tailwind CSS)
   - Columns: VIN, Brand/Model, Year, Price, Status, Actions
   - Empty state (veri yoksa)
   - Loading skeleton

3. **Filters**
   - Brand dropdown
   - Status dropdown (InStock, OnSale, Sold)
   - Search by VIN

4. **Pagination**
   - Previous/Next buttons
   - Page numbers
   - Total count göster

5. **Actions**
   - View details button (detay sayfasına gider)
   - Status badge (color-coded)

Örnek table row:
```vue
<tr>
  <td>1HGBH41JXMN109186</td>
  <td>Honda Civic 2026</td>
  <td>2026</td>
  <td>1,725,000 TRY</td>
  <td><span class="badge-success">Stokta</span></td>
  <td><button>Detay</button></td>
</tr>
```

Vue 3 Composition API kullan, TypeScript strict mode.
US-002 acceptance criteria'larını karşıla.
```

---

## 📦 Prompt 5: Vehicle Form Vue Component

### Context Dosyaları
- `docs/prompts/03-create-vue-component.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `docs/ui/vehicle-add-form.html`
- `src/frontend/pages/vehicles/new.vue` (placeholder)

### Prompt

```
Yeni araç ekleme formunu tamamla.

Hedef dosya: src/frontend/pages/vehicles/new.vue (placeholder'ı değiştir)

Context:
- docs/business/US-001-add-vehicle-to-inventory.md (acceptance criteria)
- docs/ui/vehicle-add-form.html (form mockup)
- docs/prompts/03-create-vue-component.md (pattern)

Form Alanları:
1. VIN (text input, 17 karakter, auto-uppercase)
2. Brand (select dropdown)
3. Model (text input)
4. Year (number input, max: current year)
5. Engine Type (select: Gasoline, Diesel, Electric, Hybrid)
6. Mileage (number input, default: 0)
7. Color (text input)
8. Purchase Price (number input + currency select)
9. Suggested Price (number input + currency select)
10. Transmission Type (select: Manual, Automatic, Semi-Automatic)
11. Fuel Consumption (number input, decimal)
12. Engine Capacity (number input, cc)
13. Features (multi-select checkboxes)

Validasyonlar:
- VIN: required, exactly 17 chars, no I/O/Q
- Year: required, <= current year
- Mileage: >= 0
- Prices: > 0, suggested >= purchase
- Real-time validation feedback

Form Submission:
```typescript
const submitForm = async () => {
  const response = await $fetch('/api/v1/vehicles', {
    method: 'POST',
    body: formData
  });
  
  if (response.success) {
    // Show success notification
    // Redirect to vehicle list
  }
}
```

UI Features:
- Tailwind CSS styled inputs
- Error messages (inline)
- Submit button (disabled when invalid)
- Loading state during submit
- Success/error toast notifications
- Form reset after success

Vue 3 Composition API, reactive form state, TypeScript interfaces.
US-001 tüm acceptance criteria'larını karşıla.
```

---

## 📦 Prompt 6: API Composable (useVehicleApi)

### Prompt

```
Vue composable oluştur: API operations için reusable logic.

Hedef dosya: src/frontend/composables/useVehicleApi.ts

API Endpoints:
- GET /api/v1/vehicles (list with filters)
- GET /api/v1/vehicles/{id} (single)
- POST /api/v1/vehicles (create)

Composable:
```typescript
export const useVehicleApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;

  const getVehicles = async (filters?: VehicleFilters) => {
    // Implementation with useFetch
  };

  const getVehicleById = async (id: string) => {
    // Implementation
  };

  const createVehicle = async (data: CreateVehicleRequest) => {
    // Implementation with $fetch
  };

  return {
    getVehicles,
    getVehicleById,
    createVehicle,
    // loading, error states
  };
};
```

Features:
- Loading states (ref<boolean>)
- Error handling (try/catch)
- Type-safe (TypeScript interfaces)
- Automatic base URL from config
- Return reactive refs

Ayrıca type definitions oluştur:
src/frontend/types/vehicle.ts
```typescript
export interface Vehicle {
  id: string;
  vin: string;
  brand: string;
  model: string;
  year: number;
  // ... diğer fields
}

export interface CreateVehicleRequest {
  vin: string;
  brand: string;
  // ... diğer fields
}

export interface VehicleFilters {
  page?: number;
  pageSize?: number;
  brand?: string;
  status?: string;
}

export enum VehicleStatus {
  InStock = 'InStock',
  OnSale = 'OnSale',
  Sold = 'Sold'
}
```

Nuxt 3 best practices kullan.
```

---

## 📦 Prompt 7: DbContext ve Migration

### Context Dosyaları
- `docs/prompts/02-create-ef-migration.md`
- `docs/architectural-overview/01-technology-stack.md`

### Prompt

```
Entity Framework Core DbContext oluştur.

Hedef dosya: src/backend/VehicleInventory.Infrastructure/Persistence/VehicleInventoryDbContext.cs

DbContext:
```csharp
public class VehicleInventoryDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles { get; set; }

    public VehicleInventoryDbContext(DbContextOptions<VehicleInventoryDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
    }
}
```

Entity Configuration (Fluent API):
src/backend/VehicleInventory.Infrastructure/Persistence/Configurations/VehicleConfiguration.cs
```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        // Table name
        builder.ToTable("Vehicles");
        
        // Primary key
        builder.HasKey(v => v.Id);
        
        // VIN value object mapping
        builder.OwnsOne(v => v.VIN, vin => {
            vin.Property(v => v.Value)
               .HasColumnName("VIN")
               .HasMaxLength(17)
               .IsRequired();
        });
        
        // Money value objects
        builder.OwnsOne(v => v.PurchasePrice, money => {
            money.Property(m => m.Amount).HasColumnName("PurchaseAmount");
            money.Property(m => m.Currency).HasColumnName("PurchaseCurrency");
        });
        
        // ... diğer mappings
        
        // Indexes
        builder.HasIndex(v => v.VIN.Value).IsUnique();
    }
}
```

Migration oluştur:
```bash
dotnet ef migrations add InitialCreate --project VehicleInventory.Infrastructure
```

Connection string (appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=VehicleInventory;Username=postgres;Password=postgres"
  }
}
```

DI registration (Program.cs):
```csharp
builder.Services.AddDbContext<VehicleInventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

docs/prompts/02-create-ef-migration.md pattern'ini takip et.
```

---

## 🎯 Kullanım Talimatları

### GitHub Copilot Chat ile:
1. Copilot Chat'i aç
2. İlgili dosyaları workspace'e ekle (@workspace)
3. Prompt'u yapıştır
4. Üretilen kodu incele ve düzelt
5. Test et

### Claude/ChatGPT ile:
1. Context dosyalarının içeriğini kopyala
2. Prompt'u ekle
3. Üretilen kodu IDE'ye yapıştır
4. Build ve test et

### VS Code Copilot Agent ile:
```
@workspace /new Aşağıdaki prompt'u kullanarak kod üret:
[Prompt buraya yapıştır]
```

---

## ⚠️ Önemli Notlar

- Her üretimden sonra kodu **manuel olarak inceleyin**
- **Compile errors** olup olmadığını kontrol edin
- **Spec dokümanlarına uygunluk** kontrol edin
- **Unit testler** yazın (opsiyonel ama önerilen)
- **Sonarqube** ile kod kalitesini ölçün

---

## 📚 Referans Sırası

Önerilen üretim sırası:
1. ✅ Enums
2. ✅ Vehicle Entity
3. ✅ Repository Interfaces
4. ✅ Commands & Handlers
5. ✅ Queries & Handlers
6. ✅ Validators
7. ✅ DbContext & Configurations
8. ✅ Migrations
9. ✅ API Controllers
10. ✅ Frontend Types
11. ✅ API Composables
12. ✅ Vue Components

Her adımı tamamlayıp test ettikten sonra sonrakine geçin!
