# ADR-002: Komut/Sorgu Ayrımı — CQRS ve MediatR

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Clean Architecture'ın Application katmanında use case'lerin nasıl organize edileceğine karar vermek gerekiyordu. Legacy sistemde tüm iş mantığı servis sınıflarında toplandığı için:

- Servis sınıfları onlarca metot barındırıyor ve tek sorumluluk ilkesini ihlal ediyordu
- Okuma ve yazma işlemleri aynı veri modelini paylaşıyor, bu da birini optimize etmeyi zorlaştırıyordu
- Yeni bir özellik eklemek mevcut servisleri dokunmak anlamına geliyordu (Open/Closed ihlali)
- Unit test yazmak büyük mock'lar gerektiriyordu

Hedef: her use case'i izole, tek sorumlu bir yapıya kavuşturmak.

## Değerlendirilen Seçenekler

- **Seçenek A — Geleneksel Servis Sınıfları**: `IVehicleService`, `ICustomerService` gibi interface'ler. Basit ama zamanla "god class" sorununa yol açar.
- **Seçenek B — CQRS (Command Query Responsibility Segregation)**: Yazma (Command) ve okuma (Query) işlemlerini ayrı modellere ve handler'lara bölme. MediatR ile kolayca uygulanabilir.
- **Seçenek C — Minimal API + Endpoint Handler**: Her endpoint kendi handler'ını çağırır. Katman sınırları belirsizleşebilir.

## Karar

**CQRS deseni, MediatR kütüphanesi** aracılığıyla uygulandı.

Her use case kendi sınıfında tanımlanır:

```
Commands/
  AddVehicle/
    AddVehicleCommand.cs        → IRequest<Guid>
    AddVehicleCommandHandler.cs → IRequestHandler<AddVehicleCommand, Guid>

Queries/
  GetVehicles/
    GetVehiclesQuery.cs         → IRequest<IEnumerable<VehicleDto>>
    GetVehiclesQueryHandler.cs  → IRequestHandler<GetVehiclesQuery, ...>
```

Controller, doğrudan handler'a değil **MediatR.ISender** arayüzüne bağımlıdır:

```csharp
var result = await _sender.Send(new AddVehicleCommand(...));
```

Bu tercih yapıldı çünkü:
- Her handler yalnızca bir use case'e odaklanır (Single Responsibility)
- Yeni özellik eklemek mevcut kodu değiştirmeyi gerektirmez (Open/Closed)
- Handler bağımlılıkları constructor injection ile net biçimde görünür
- MediatR pipeline (logging, validation, transaction) cross-cutting concern'leri merkezi olarak yönetir
- AI destekli kod üretimi için her handler izole bir bağlamdır

## Sonuçlar

### Olumlu Sonuçlar

- Her use case tek bir dosyada, okunması kolay ve test edilmesi basit
- Okuma sorguları için farklı veri modeli (DTO) ve optimizasyon (Dapper) kullanılabilir
- MediatR pipeline behavior'ları ile loglama, doğrulama ve transaction yönetimi merkezi hale gelir
- Handler'lar kolayca mock'lanabilir; controller testleri sade kalır
- Feature-by-Feature organizasyon sayesinde yeni geliştiriciler ilgili kodu hızla bulur

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Basit CRUD için çok sayıda dosya oluşturmak gerekir
- MediatR'ın reflection tabanlı yapısı, compile-time bağlantının kaybolması anlamına gelir (handler bulunamazsa çalışma zamanında hata)
- Yeni başlayanlar için CQRS terminolojisi (Command, Query, Handler) ek öğrenme gerektirir

## İlgili ADR'ler

- [ADR-001](ADR-001-clean-architecture.md) — Clean Architecture (genel mimari çerçeve)
- [ADR-009](ADR-009-fluentvalidation.md) — FluentValidation (MediatR pipeline ile entegrasyon)
