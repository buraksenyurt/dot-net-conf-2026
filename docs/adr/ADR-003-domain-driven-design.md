# ADR-003: Domain Modelleme Yaklaşımı — Domain-Driven Design (DDD)

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Bayi Yönetim Sistemi (DMS) terminolojisi ve iş kuralları karmaşık ve çok sayıda iş birimini kapsıyor. Legacy sistemde bu karmaşıklık veritabanı şemasına ve stored procedure'lara gömülmüştü:

- İş kuralları veritabanı trigger'larına yayılmıştı; C# kodunda görünmüyordu
- Aynı kavram (müşteri, araç, opsiyon) farklı bağlamlarda farklı anlamlar taşıyordu
- Ubiquitous Language yoktu; geliştiriciler, analistler ve kullanıcılar farklı terimler kullanıyordu
- Varlıkların durum geçişleri (araç: stokta → satışta → rezerve → satıldı) dağınık şekilde yönetiliyordu

## Değerlendirilen Seçenekler

- **Seçenek A — Anemic Domain Model**: Entity'ler yalnızca özellikler barındırır; iş mantığı servis sınıflarında. Basit ama iş kurallarını modellemek zorlaşır.
- **Seçenek B — Domain-Driven Design (Taktik Desenler)**: Entity, Value Object, Aggregate, Domain Event gibi yapı taşları kullanılır. İş kuralları domain katmanında yaşar.
- **Seçenek C — Functional Domain Modeling**: Değişmez (immutable) tipler ve pure fonksiyonlarla modelleme. F# ekosisteminde yaygın, C# için alışılmadık.

## Karar

**DDD taktik desenleri** uygulandı. Öne çıkan kararlar:

### Aggregate'ler
- `Vehicle` — Araç envanteri; `VehicleOption` bağımlı aggregate olarak değil bağımsız ele alındı
- `Customer` — Bireysel veya kurumsal müşteri
- `ServiceAdvisor` — Yetkili kullanıcı
- `VehicleOption` — Satın alma opsiyonu (1–30 gün)

### Value Object'ler
- **VIN**: ISO 3779 standartına göre doğrulama + check digit (9. pozisyon). Değişmez, eşitlik değer bazlı.
- **Email**: RFC 5322 formatı, normalize edilmiş (küçük harfe çevrilir). Değişmez.
- **Money**: Miktar + ISO 4217 para birimi kodu. Farklı para birimleri arası toplama engellenir.

### Durum Makinesi (State Machine)
`Vehicle.Status` için geçişler domain katmanında metotlarla zorunlu kılındı:
```
InStock → OnSale → Reserved → Sold
```
Geçersiz geçişler `DomainException` fırlatır.

Bu tercih yapıldı çünkü:
- Ubiquitous Language: DMS terminolojisi doğrudan kod sınıf ve metot adlarına yansır
- İş kuralları (VIN check digit, opsiyon süresi, durum geçişi) domain dışına sızmaz
- Value Object'ler immutable olduğundan thread-safe ve yan etkisiz
- Analistler, QA ve geliştiriciler aynı dili konuşur

## Sonuçlar

### Olumlu Sonuçlar

- Domain kuralları tek yerde, test etmek kolay; `VinTests`, `MoneyTests`, `CustomerTests` sade birim testleri
- Yeni iş gereksinimi eklendiğinde etkilenen aggregate açıkça bellidir
- AI destekli geliştirmede domain modeli spec dokümanı olarak kullanılabilir
- Anemic model tuzağı (iş mantığının servislere kayması) engellenir

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- DDD terminolojisi (Aggregate, Bounded Context, Value Object) ek öğrenme gerektirir
- Value Object'lerin EF Core ile eşlenmesi ek konfigürasyon gerektirir (`OwnsOne`, `ComplexProperty`)
- Küçük PoC için bazı DDD yapıları gereksiz görünebilir; pragmatik tutuldu (Bounded Context sınırları genişletilmedi)

## İlgili ADR'ler

- [ADR-001](ADR-001-clean-architecture.md) — Clean Architecture (domain katmanının yeri)
- [ADR-005](ADR-005-ef-core-dapper.md) — EF Core (value object eşleme stratejisi)
