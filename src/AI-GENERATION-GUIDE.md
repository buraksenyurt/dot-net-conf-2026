# Demo Uygulaması - AI ile Kod Üretimi Rehberi

Bu doküman, demo uygulamasının kalan kısımlarını AI kullanarak nasıl tamamlayacağınızı açıklar.

## 📦 Mevcut Durum

### ✅ Tamamlanan
- **Spec Dokümanları** (`docs/`) - Tüm user story'ler, domain modeller, UI mockup'ları hazır
- **Backend Temelleri** (.NET 9 Clean Architecture, Value Objects)
- **Frontend Şablonu** (Vue 3 + Vite, Router, Layout, TypeScript)

### 🤖 AI ile Tamamlanacak
- Backend: Entity, Commands/Queries, DbContext, API Controllers
- Frontend: VehicleList, VehicleForm, API Composables, Types

## 🎯 AI ile Kod Üretimi Adımları

### 1. Backend Entity ve Enums

**Referans Dokümanlar:**
- `docs/domain-model/entity-vehicle.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `docs/architectural-overview/02-coding-standards.md`

**AI Prompt:**
```
docs/domain-model/entity-vehicle.md dokümanını kullanarak:
- src/backend/VehicleInventory.Domain/Entities/Vehicle.cs entity'sini oluştur
- src/backend/VehicleInventory.Domain/Enums/ klasöründe gerekli enum'ları ekle
- DDD prensiplerine uy, VIN ve Money value object'lerini kullan
```

### 2. Application Layer (CQRS)

**Referans Dokümanlar:**
- `docs/prompts/01-create-api-endpoint.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `docs/business/US-002-list-vehicles.md`

**AI Prompt:**
```
docs/prompts/01-create-api-endpoint.md promptunu kullanarak:
- AddVehicleCommand ve AddVehicleCommandHandler oluştur
- GetVehiclesQuery ve GetVehiclesQueryHandler oluştur
- FluentValidation ile validatorlar ekle
- src/backend/VehicleInventory.Application/ klasörüne yerleştir
```

### 3. Infrastructure Layer (DbContext)

**Referans Dokümanlar:**
- `docs/prompts/02-create-ef-migration.md`
- `docs/architectural-overview/01-technology-stack.md`

**AI Prompt:**
```
docs/prompts/02-create-ef-migration.md promptunu kullanarak:
- VehicleInventoryDbContext oluştur
- Vehicle entity configuration (Fluent API)
- Value Object mapping (VIN, Money)
- PostgreSQL bağlantısı
- Initial migration oluştur
```

### 4. API Layer (Controllers)

**AI Prompt:**
```
docs/prompts/01-create-api-endpoint.md kullanarak:
- VehiclesController oluştur
- POST /api/v1/vehicles endpoint
- GET /api/v1/vehicles endpoint
- Swagger/OpenAPI dokümantasyonu
- Error handling middleware
```

### 5. Frontend - VehicleList Component

**Referans Dokümanlar:**
- `docs/prompts/03-create-vue-component.md`
- `docs/business/US-002-list-vehicles.md`
- `docs/ui/vehicle-list.html`

**AI Prompt:**
```
docs/prompts/03-create-vue-component.md kullanarak:
- src/frontend/pages/vehicles/index.vue sayfasını tamamla
- API'den araç listesi çek
- Tablo görünümü, filtreleme, pagination
- Tailwind CSS ile styling
- TypeScript tip tanımları
```

### 6. Frontend - VehicleForm Component

**Referans Dokümanlar:**
- `docs/prompts/03-create-vue-component.md`
- `docs/business/US-001-add-vehicle-to-inventory.md`
- `docs/ui/vehicle-add-form.html`

**AI Prompt:**
```
docs/prompts/03-create-vue-component.md kullanarak:
- src/frontend/pages/vehicles/new.vue sayfasını tamamla
- Reactive form (VIN, marka, model, fiyat, vb)
- Client-side validasyon
- API POST request
- Success/error notifications
- docs/business/US-001 acceptance criteria'larına uy
```

### 7. Frontend - API Composables

**AI Prompt:**
```
src/frontend/composables/useVehicleApi.ts oluştur:
- useFetch ile API calls
- CRUD operations (list, create, get, update)
- Error handling
- Loading states
- TypeScript types
```

### 8. Frontend - Type Definitions

**AI Prompt:**
```
src/frontend/types/vehicle.ts oluştur:
- Vehicle interface
- CreateVehicleRequest type
- VehicleListResponse type
- Enums (VehicleStatus, EngineType, etc.)
```

## 🔧 Çalıştırma

### Backend
```bash
cd src/backend
dotnet restore
dotnet ef database update --project VehicleInventory.Infrastructure
dotnet run --project VehicleInventory.API
```

### Frontend
```bash
cd src/frontend
npm install
npm run dev
```

### Docker
```bash
# docker-compose.yml güncellenmeli
docker-compose up -d
```

## 📝 Notlar

- Her AI üretimi sonrası kodu inceleyin ve test edin
- Spec dokümanlarına sadık kalın
- DDD ve Clean Architecture prensiplerine uyun
- TypeScript strict mode açık, tiplere dikkat edin

## 🎬 Demo Senaryosu

1. Backend API'yi başlat
2. Frontend'i başlat
3. Ana sayfayı aç (http://localhost:3000)
4. "Yeni Araç Ekle" ile form doldur
5. "Araç Listesi" ile eklenen araçları görüntüle
6. Sonarqube MCP Server ile kod kalitesini kontrol et
7. Sunumda AI prompt'ları ve üretilen kodları göster

## 📚 Ek Kaynaklar

- Main README.md - Proje genel bilgileri ve diyagramlar
- docs/README.md - Spec dokümanları kullanım kılavuzu
- src/backend/README.md - Backend proje yapısı (oluşturulmalı)
- src/frontend/README.md - Frontend proje yapısı
