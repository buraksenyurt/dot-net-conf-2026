# ADR-005: Veri Erişim Stratejisi — EF Core + Dapper (Hibrit Yaklaşım)

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Veri erişim katmanında iki temel gereksinim çatışıyordu:

1. **Yazma işlemleri**: Domain nesnelerinin (aggregate, value object) veritabanıyla eşlenmesi, migration yönetimi ve ORM'in sağladığı DDD desteği.
2. **Okuma işlemleri**: Listeleme, filtreleme, arama gibi sorgularda performans kritik; ORM'in ürettiği SQL her zaman optimal değildir.

Legacy sistemde ADO.NET ve inline SQL kullanılıyordu. Bu yaklaşım esnek ama bakımı zor, SQL injection riskli ve migration yönetimi elle yapılıyordu.

## Değerlendirilen Seçenekler

- **Seçenek A — Yalnızca EF Core**: ORM tüm erişimi tek elden yönetir. Kolay migration, DDD desteği. Ancak karmaşık okuma sorgularında N+1 ve gereksiz JOIN sorunları yaşanabilir.
- **Seçenek B — Yalnızca Dapper**: Tam kontrol, yüksek performans. Ancak migration yönetimi, value object eşlemesi ve DDD aggregate takibi elle yapılmalı.
- **Seçenek C — Hibrit (EF Core yazma + Dapper okuma)**: CQRS ile mükemmel uyum. Command tarafı EF Core, Query tarafı Dapper kullanır.
- **Seçenek D — Yalnızca ADO.NET**: Legacy yaklaşım; çok fazla boilerplate, güvenlik gerektiriyor.

## Karar

**Hibrit yaklaşım** seçildi: **EF Core 9 yazma işlemleri için, Dapper okuma işlemleri için.**

```
Commands (Yazma)  →  EF Core + DbContext  →  PostgreSQL
Queries  (Okuma)  →  Dapper + IDbConnection  →  PostgreSQL
```

### EF Core Kullanım Alanları
- Aggregate kayıt, güncelleme, silme
- Value object eşlemesi (`OwnsOne` → `Email`, `Money`, `VIN`)
- Migration yönetimi (kod öncelikli şema yönetimi)
- Change Tracking ile tutarlı aggregate güncellemesi

### Dapper Kullanım Alanları
- Araç listesi (filtreleme + sayfalama + sıralama)
- Müşteri arama (isim, TC, telefon)
- Servis danışmanı dashboard verisi (aktif opsiyonlar, yaklaşan bitiş tarihleri)
- Raporlama sorguları

Bu tercih yapıldı çünkü:
- CQRS'in Command/Query ayrımıyla doğal uyum
- Okuma için elle yazılmış SQL → optimum sorgu planı ve tam kontrol
- Yazma için EF Core → migration, value object, change tracking avantajları
- Her iki kütüphane de `Npgsql` sağlayıcısıyla PostgreSQL'de sorunsuz çalışır

## Sonuçlar

### Olumlu Sonuçlar

- Okuma sorguları tam optimize edilmiş SQL kullanır; EF Core'un üreteceği gereksiz JOIN ve SELECT * engellenmiştir
- EF Core migration'ları ile şema geçmişi versiyonlanır
- Value object'ler (`OwnsOne`) EF Core ile şeffaf biçimde tek tabloda saklanır
- Her query handler, join ve filtreleme mantığını açıkça SQL'de ifade eder; gizli ORM davranışı yoktur

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- İki farklı kütüphanede yetkinlik gerekir (EF Core ve Dapper)
- Dapper sorguları string tabanlıdır; refactoring sırasında SQL tablo/kolon adları elle takip edilmelidir
- EF Core ile Dapper arasında transaction koordinasyonu yapılmalıdır (`IDbTransaction` paylaşımı)

## İlgili ADR'ler

- [ADR-001](ADR-001-clean-architecture.md) — Clean Architecture (Infrastructure katmanının yeri)
- [ADR-002](ADR-002-cqrs-mediatr.md) — CQRS (Command/Query ayrımı veri erişim stratejisini şekillendirir)
- [ADR-004](ADR-004-postgresql.md) — PostgreSQL (hedef veritabanı)
