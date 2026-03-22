# ADR-001: Backend Mimari Deseni — Clean Architecture

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Modernizasyon projesinin PoC (Proof of Concept) aşamasında yeni bir backend çözümü tasarlamak gerekiyordu. Legacy sistemdeki temel sorunlardan biri, iş mantığının veri erişim katmanı ve UI katmanıyla sıkı sıkıya bağlı olmasıydı (tight coupling). Bu durum:

- Birim testini neredeyse imkânsız kılıyordu
- Teknoloji değişikliklerini son derece maliyetli hale getiriyordu
- Yeni geliştiricilerin sistemi anlamasını zorlaştırıyordu
- Bağımlılık yönetimini karmaşık bir hale getiriyordu

Hedef, bağımsız olarak geliştirilebilen, test edilebilen ve değiştirilebilen bir katmanlı mimari seçmekti.

## Değerlendirilen Seçenekler

- **Seçenek A — N-Tier (Katmanlı) Mimari**: Klasik Presentation → Business Logic → Data Access katmanları. Legacy sistemde halihazırda kullanılıyor, ancak sıkı bağlılık sorunlarını çözmüyor.
- **Seçenek B — Clean Architecture**: Robert C. Martin'in önerdiği, bağımlılıkların içe doğru aktığı (Dependency Inversion), domain'in merkeze alındığı mimari. Framework ve altyapı bağımlılıkları en dış katmanda tutulur.
- **Seçenek C — Vertical Slice Architecture**: Her özelliğin kendi katmanını oluşturduğu, feature-centric yaklaşım. CQRS ile iyi uyum sağlar ancak küçük ekipler için öğrenme eğrisi yüksektir.

## Karar

**Clean Architecture** seçildi. Proje 4 katmanlı bir yapıya bölündü:

```
VehicleInventory.Domain         → İş kuralları, entity'ler, value object'ler (dış bağımlılık yok)
VehicleInventory.Application    → Use case'ler, CQRS handler'lar, interface'ler
VehicleInventory.Infrastructure → EF Core, repository implementasyonları, dış servisler
VehicleInventory.API            → Controller'lar, middleware, DI kayıtları
```

**Bağımlılık yönü**: API → Infrastructure → Application → Domain (Domain'e bağımlılık yalnızca içten dışa)

Bu tercih yapıldı çünkü:
- Domain katmanı saf C# — hiçbir framework bağımlılığı yok
- Application katmanı interface'ler üzerinden çalışır; altyapı değiştirildiğinde etkilenmez
- Test edilebilirlik maksimum seviyede: domain ve application katmanları in-memory veya mock ile test edilebilir
- AI destekli kod üretimi için açık ve tahmin edilebilir bir yapı sağlar

## Sonuçlar

### Olumlu Sonuçlar

- Domain mantığı framework ve veritabanı bağımlılıklarından tamamen izole
- Her katman bağımsız olarak birim testi yazılabilir
- Yeni geliştiriciler katman sorumluluklarını hızla kavrar
- Altyapı bileşenleri (PostgreSQL → başka DB, Keycloak → başka auth) domain katmanını etkilemeden değiştirilir
- AI araçlarına (GitHub Copilot, Claude) net bağlam sağlar; kod üretimi tutarlı kalır

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Basit CRUD işlemleri için fazladan dosya ve katman gerektirir («over-engineering» riski)
- İlk setup maliyeti yüksektir; her yeni feature için Command/Query/Handler/Repository üçlüsü oluşturulmalıdır
- Junior geliştiriciler için öğrenme eğrisi mevcuttur

## İlgili ADR'ler

- [ADR-002](ADR-002-cqrs-mediatr.md) — CQRS ve MediatR (Application katmanının iç organizasyonu)
- [ADR-003](ADR-003-domain-driven-design.md) — DDD (Domain katmanının tasarım prensipleri)
- [ADR-005](ADR-005-ef-core-dapper.md) — EF Core + Dapper (Infrastructure katmanının veri erişim stratejisi)
