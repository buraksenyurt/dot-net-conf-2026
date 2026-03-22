# ADR-008: Yapılandırılmış Loglama — Serilog

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy DMS sisteminde loglama, `System.Diagnostics.Trace` ve dosya tabanlı metin logları ile yapılıyordu. Bu yaklaşımın sorunları:

- Loglar yapılandırılmamış düz metin; makine tarafından ayrıştırılması zordu
- Merkezi log toplama aracıyla entegrasyon yoktu
- Belirli bir işlem veya kullanıcı için log izleme neredeyse imkânsızdı (correlation yoktu)
- Üretimde performans sorunlarını teşhis etmek çok uzun süre alıyordu

Modern loglama altyapısından beklentiler:
- JSON formatında yapılandırılmış log (makine okunabilir)
- Korelasyon kimliği (correlation ID) ile request takibi
- Çoklu hedef (konsol, dosya, ileride Elastic/Seq/Datadog)
- ASP.NET Core pipeline ile kolay entegrasyon

## Değerlendirilen Seçenekler

- **Seçenek A — Microsoft.Extensions.Logging (yerleşik)**: Standart, entegre. Ancak yapılandırılmış loglama desteği sınırlı; sink eklemek için ek kütüphane gerekir.
- **Seçenek B — Serilog**: .NET ekosisteminin en olgun yapılandırılmış loglama kütüphanesi. Zengin sink ekosistemi, message template, destructured object logging.
- **Seçenek C — NLog**: Olgun, esnek. Serilog kadar modern yapılandırılmış loglama desteği yok.
- **Seçenek D — OpenTelemetry (yalnızca)**: Standartlara dayalı, vendor-agnostic. Ancak kurulum karmaşıklığı PoC için fazla; ileride eklenebilir.

## Karar

**Serilog** seçildi. `ILogger<T>` soyutlamasıyla entegre kullanıldı.

`Program.cs` konfigürasyonu:
```csharp
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day));
```

Message template kullanımı (string interpolation yerine):
```csharp
_logger.LogInformation("Araç eklendi: {VehicleId} — VIN: {Vin}", vehicle.Id, vehicle.VIN);
```

Bu tercih yapıldı çünkü:
- JSON sink ile log analitiği araçlarına (Seq, Elastic) ileride kolayca bağlanılabilir
- `FromLogContext()` ile HTTP request bilgileri (method, path, status) otomatik eklenir
- Destructuring `{@object}` sözdizimi ile nesne detayları log kaydına yazılır
- `Microsoft.Extensions.Logging` interface'i korunduğu için bağımlılık minimum; test'te `NullLogger` kullanılabilir

## Sonuçlar

### Olumlu Sonuçlar

- Her log kaydı JSON formatında; grep veya analitik araçlarıyla sorgulanabilir
- Correlation ID ile tek bir kullanıcı işlemi baştan sona izlenebilir
- Rolling file sink ile disk yönetimi otomatik
- `ILogger<T>` injectable olduğu için handler ve servisler test ortamında silinik logger ile çalışabilir
- OpenTelemetry ile entegrasyon yol haritasına eklenmiş (Serilog OpenTelemetry sink mevcut)

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Serilog konfigürasyonu `appsettings.json`'da ayrıca tanımlanmalı
- Production log yönetimi (log rotation, arşivleme) ek altyapı gerektirir
- JSON log formatı konsol okumayı zorlaştırır; geliştirici deneyimi için `Serilog.Sinks.Console` ile anlamlı format ayarlanmalı

## İlgili ADR'ler

- [ADR-001](ADR-001-clean-architecture.md) — Clean Architecture (loglama cross-cutting concern olarak Infrastructure katmanında)
- [ADR-012](ADR-012-github-actions-cicd.md) — GitHub Actions (CI/CD pipeline'da log artefact'ları)
