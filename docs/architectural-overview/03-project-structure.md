# Proje Yapısı

## Backend Proje Organizasyonu

### Clean Architecture Katmanları

```
VehicleManagement/
│
├── src/
│   │
│   ├── VehicleManagement.Domain/              # Domain Layer
│   │   ├── Entities/
│   │   │   ├── Vehicle.cs
│   │   │   ├── Customer.cs
│   │   │   └── Dealer.cs
│   │   ├── ValueObjects/
│   │   │   ├── VIN.cs
│   │   │   ├── Money.cs
│   │   │   ├── Email.cs
│   │   │   └── Address.cs
│   │   ├── Aggregates/
│   │   │   └── VehicleInventory.cs
│   │   ├── Enums/
│   │   │   ├── CustomerType.cs
│   │   │   └── VehicleStatus.cs
│   │   ├── Events/
│   │   │   ├── VehicleAddedEvent.cs
│   │   │   └── VehicleSoldEvent.cs
│   │   ├── Exceptions/
│   │   │   └── VehicleDomainException.cs
│   │   └── Interfaces/
│   │       ├── IVehicleRepository.cs
│   │       └── ICustomerRepository.cs
│   │
│   ├── VehicleManagement.Application/         # Application Layer
│   │   ├── Commands/
│   │   │   ├── AddVehicle/
│   │   │   │   ├── AddVehicleCommand.cs
│   │   │   │   ├── AddVehicleCommandHandler.cs
│   │   │   │   └── AddVehicleCommandValidator.cs
│   │   │   ├── CreateCustomer/
│   │   │   │   ├── CreateCustomerCommand.cs
│   │   │   │   └── CreateCustomerCommandHandler.cs
│   │   │   └── UpdateVehicleStatus/
│   │   │       ├── UpdateVehicleStatusCommand.cs
│   │   │       └── UpdateVehicleStatusCommandHandler.cs
│   │   ├── Queries/
│   │   │   ├── GetVehicles/
│   │   │   │   ├── GetVehiclesQuery.cs
│   │   │   │   ├── GetVehiclesQueryHandler.cs
│   │   │   │   └── VehicleDto.cs
│   │   │   ├── GetVehicleByVin/
│   │   │   │   ├── GetVehicleByVinQuery.cs
│   │   │   │   └── GetVehicleByVinQueryHandler.cs
│   │   │   ├── GetCustomers/
│   │   │   │   ├── GetCustomersQuery.cs
│   │   │   │   ├── GetCustomersQueryHandler.cs
│   │   │   │   └── CustomerDto.cs
│   │   │   └── GetCustomerById/
│   │   │       ├── GetCustomerByIdQuery.cs
│   │   │       └── GetCustomerByIdQueryHandler.cs
│   │   ├── Mappings/
│   │   │   └── VehicleProfile.cs
│   │   ├── Validators/
│   │   │   └── VinValidator.cs
│   │   └── Interfaces/
│   │       └── IVehicleService.cs
│   │
│   ├── VehicleManagement.Infrastructure/       # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Repositories/
│   │   │   │   └── VehicleRepository.cs
│   │   │   ├── Configurations/
│   │   │   │   └── VehicleConfiguration.cs
│   │   │   │   └── CustomerConfiguration.cs
│   │   │   └── Migrations/
│   │   ├── ExternalServices/
│   │   │   └── KeycloakAuthService.cs
│   │   ├── Messaging/
│   │   │   └── RabbitMqPublisher.cs
│   │   └── DependencyInjection.cs
│   │
│   └── VehicleManagement.Api/                  # Presentation Layer
│       ├── Controllers/
│       │   ├── VehiclesController.cs
│       │   └── CustomersController.cs
│       ├── Filters/
│       │   ├── GlobalExceptionFilter.cs
│       │   └── ValidationFilter.cs
│       ├── Middleware/
│       │   ├── RequestLoggingMiddleware.cs
│       │   └── PerformanceMiddleware.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Program.cs
│
├── tests/
│   ├── VehicleManagement.Domain.Tests/
│   │   └── Entities/
│   │       └── VehicleTests.cs
│   ├── VehicleManagement.Application.Tests/
│   │   └── Commands/
│   │       └── AddVehicleCommandHandlerTests.cs
│   └── VehicleManagement.Api.Tests/
│       └── Controllers/
│           └── VehiclesControllerTests.cs
│
├── VehicleManagement.sln
└── README.md
```

## Frontend Proje Organizasyonu

```
vehicle-management-ui/
│
├── assets/                          # Statik dosyalar
│   ├── css/
│   ├── images/
│   └── fonts/
│
├── components/                      # Vue bileşenleri (Atomic Design)
│   ├── atoms/
│   │   ├── VButton.vue
│   │   ├── VInput.vue
│   │   └── VBadge.vue
│   ├── molecules/
│   │   ├── VehicleCard.vue
│   │   ├── SearchBar.vue
│   │   └── StatusFilter.vue
│   ├── organisms/
│   │   ├── VehicleList.vue
│   │   ├── VehicleForm.vue
│   │   └── NavigationBar.vue
│   └── templates/
│       └── DashboardLayout.vue
│
├── composables/                     # Yeniden kullanılabilir mantık
│   ├── useVehicleInventory.ts
│   ├── useAuth.ts
│   ├── useNotification.ts
│   └── useValidation.ts
│
├── layouts/                         # Nuxt layouts
│   ├── default.vue
│   └── auth.vue
│
├── pages/                           # Nuxt sayfa route'ları
│   ├── index.vue                    # Ana sayfa
│   ├── vehicles/
│   │   ├── index.vue                # Araç listesi
│   │   ├── [id].vue                 # Araç detay
│   │   └── add.vue                  # Yeni araç ekleme
│   └── login.vue
│
├── plugins/                         # Nuxt plugins
│   ├── api.ts
│   └── i18n.ts
│
├── stores/                          # Pinia stores
│   ├── vehicle.ts
│   ├── auth.ts
│   └── notification.ts
│
├── types/                           # TypeScript type definitions
│   ├── vehicle.ts
│   ├── api.ts
│   └── index.ts
│
├── utils/                           # Yardımcı fonksiyonlar
│   ├── validators.ts
│   ├── formatters.ts
│   └── constants.ts
│
├── middleware/                      # Route middleware
│   ├── auth.ts
│   └── permission.ts
│
├── nuxt.config.ts
├── tsconfig.json
├── package.json
└── README.md
```

## Veritabanı Migration Yapısı

```
VehicleManagement.Infrastructure/
└── Persistence/
    └── Migrations/
        ├── 20260201_Initial.cs
        ├── 20260202_AddVehicleStatus.cs
        └── 20260203_AddDealerRelation.cs
```

### Migration İsimlendirme
- Format: `YYYYMMDD_DescriptiveName.cs`
- Açıklayıcı ve kısa isimler
- Değişikliği yansıtan isimler

## API Endpoint Yapısı

```
/api/v1/
├── vehicles/                     # Araç yönetimi
│   ├── GET    /                  # Tüm araçları listele
│   ├── GET    /{id}              # Tek araç detay
│   ├── GET    /vin/{vin}         # VIN'e göre araç
│   ├── POST   /                  # Yeni araç ekle
│   ├── PUT    /{id}              # Araç güncelle
│   ├── PATCH  /{id}/status       # Durum güncelle
│   └── DELETE /{id}              # Araç sil
│
├── dealers/                      # Bayi yönetimi
│   ├── GET    /
│   ├── GET    /{id}
│   ├── POST   /
│   └── PUT    /{id}
│
└── health                        # Health check
```

### Versioning
- URL-based versioning: `/api/v1/`
- Major değişikliklerde versiyon artışı
- Backward compatibility mümkün olduğunca korunmalı

## Ortam Yapılandırması

### Backend Configuration Files
```
appsettings.json              # Genel ayarlar
appsettings.Development.json  # Dev ortamı
appsettings.Staging.json      # Test ortamı
appsettings.Production.json   # Prod ortamı
```

### Frontend Environment Files
```
.env                          # Genel değişkenler
.env.development              # Dev ortamı
.env.staging                  # Test ortamı
.env.production               # Prod ortamı
```

## Docker Yapısı

```
docker/
├── docker-compose.yml              # Geliştirme ortamı
├── docker-compose.prod.yml         # Production ortamı
├── api/
│   └── Dockerfile
└── ui/
    └── Dockerfile
```

## CI/CD Pipeline Yapısı

```
.github/
└── workflows/
    ├── backend-ci.yml              # Backend build ve test
    ├── frontend-ci.yml             # Frontend build ve test
    ├── deploy-staging.yml          # Staging deployment
    └── deploy-production.yml       # Production deployment
```
