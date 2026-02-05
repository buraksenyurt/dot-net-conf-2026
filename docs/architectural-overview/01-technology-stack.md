# Teknoloji Altyapısı

## Backend Stack

### Framework ve Dil
- **.NET 8.0** - Ana framework
- **C# 12** - Programlama dili
- **ASP.NET Core Web API** - REST API geliştirme

### Veri Erişim
- **Entity Framework Core 8.0** - ORM
- **Dapper** - Performans kritik sorgular için
- **PostgreSQL 15** - İlişkisel veritabanı

### Güvenlik ve Kimlik Yönetimi
- **Keycloak** - Authentication ve Authorization
- **JWT Bearer Token** - API güvenliği

### Dependency Injection
- **.NET Core DI Container** - Yerleşik DI sistemi

### Logging ve Monitoring
- **Serilog** - Yapılandırılmış loglama
- **OpenTelemetry** - Distributed tracing

## Frontend Stack

### Framework ve Library
- **Vue.js 3** - Progressive JavaScript framework
- **Nuxt 3** - Vue meta-framework
- **TypeScript** - Tip güvenli JavaScript

### UI Component Library
- **Kurum İçi Geliştirilen Bileşen Kütüphanesi**
- **Tailwind CSS** - Utility-first CSS framework

### State Management
- **Pinia** - Vue state management

## Altyapı ve DevOps

### Versiyon Kontrol
- **Git** - Versiyon kontrol sistemi
- **GitHub** - Repository hosting
- **Git Flow** - Branching stratejisi

### CI/CD
- **GitHub Actions** - Otomatik build ve deployment
- **GitOps** - Deployment stratejisi

### Kod Kalitesi
- **SonarQube** - Statik kod analizi
- **ESLint** - JavaScript/TypeScript linting
- **Prettier** - Kod formatlama

## Mimari Prensipler

### Backend
- **Clean Architecture** - Katmanlı mimari yapısı
- **CQRS Pattern** - Command ve Query ayrımı
- **Domain-Driven Design** - Domain odaklı tasarım
- **Repository Pattern** - Veri erişim soyutlaması
- **Unit of Work Pattern** - Transaction yönetimi

### Frontend
- **Composition API** - Vue 3 modern API
- **Component-Based Architecture** - Yeniden kullanılabilir bileşenler
- **Atomic Design** - UI bileşen hiyerarşisi

## İletişim Protokolleri

### API
- **REST API** - Standart HTTP tabanlı API
- **gRPC** - Performans kritik servis iletişimi (opsiyonel)
- **JSON** - Veri formatı

### Mesajlaşma
- **RabbitMQ** - Asenkron mesajlaşma (entegrasyon senaryoları için)

## Test Stratejisi

### Backend
- **xUnit** - Unit test framework
- **FluentAssertions** - Test assertion library
- **Moq** - Mocking framework
- **TestContainers** - Integration test için container'lar

### Frontend
- **Vitest** - Unit test framework
- **Vue Test Utils** - Vue component testing
- **Playwright** - E2E testing

## Geliştirme Ortamı

### IDE ve Araçlar
- **Visual Studio Code** - Tercih edilen IDE
- **JetBrains Rider** - .NET geliştirme (opsiyonel)
- **GitHub Copilot** - AI asistan

### Konteynerizasyon
- **Docker** - Yerel geliştirme ortamı
- **Docker Compose** - Multi-container orchestration
