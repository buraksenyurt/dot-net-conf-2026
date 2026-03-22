# ADR-010: REST API Versiyonlama Stratejisi — URL Tabanlı Versiyonlama

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Bayi Yönetim Sistemi API'si, ilerleyen dönemde birden fazla frontend (web, mobil, üçüncü taraf) tarafından tüketilecektir. Bu senaryoda breaking change'ler kaçınılmazdır. API versiyonlama stratejisi belirlenmeden, tüketici uygulama bozulmalarını önlemek mümkün değildir.

Mevcut PoC kapsamında `v1` asgari düzeyde tanımlanmakta; ancak mimarinin başından itibaren versiyonlamayı desteklemesi hedeflenmektedir.

## Değerlendirilen Seçenekler

- **Seçenek A — URL yolu tabanlı (`/api/v1/vehicles`)**: Açık, önbelleklenebilir, tarayıcı ve araçlarla uyumlu. URL'ye bakarak versiyon anlaşılır.
- **Seçenek B — Query string tabanlı (`/api/vehicles?version=1`)**: Basit ama okunabilirliği düşük; REST tasarım ilkelerine pek uymaz.
- **Seçenek C — Header tabanlı (`Api-Version: 1`)**: URL temiz kalır. Ancak tarayıcı testleri ve URL paylaşımı zorlaşır; önbellekleme karmaşıklaşabilir.
- **Seçenek D — Media type tabanlı (Content negotiation)**: En RESTful yaklaşım ama kurulum karmaşık, araç desteği sınırlı.

## Karar

**URL yolu tabanlı versiyonlama** seçildi: `/api/v1/`

```
GET  /api/v1/vehicles
POST /api/v1/vehicles
GET  /api/v1/vehicles/{id}
GET  /api/v1/customers
```

`Asp.Versioning.Mvc` kütüphanesi ile uygulandı:

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

Controller düzeyinde:
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```

Bu tercih yapıldı çünkü:
- Swagger/OpenAPI dokümantasyonunda versiyon grupları net biçimde ayrılır
- Frontend geliştirici versiyonu URL'den hemen görür; header ayarı gerekmez
- API test araçları (Postman, .http dosyaları) ile kolayca çalışılır
- Proxy ve API gateway kuralları URL prefix'e göre kolayca yazılır

## Sonuçlar

### Olumlu Sonuçlar

- API tüketicileri hangi versiyonu çağırdıklarını açıkça bilir
- `v1` ve `v2` aynı anda sunulabilir; kademeli geçiş mümkün
- IDE ve Swagger desteği mükemmel; API keşfi kolaylaşır
- CI/CD'de farklı versiyon uç noktaları bağımsız test edilebilir

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- URL değiştiğinde bookmark ve hardcoded URL'ler güncellenmeli
- Çok sayıda versiyon zamanla bakım yükü oluşturabilir; deprecation politikası belirlenmelidir
- Route şablonu uzar: `/api/v{version:apiVersion}/[controller]`

## İlgili ADR'ler

- [ADR-007](ADR-007-vue3-frontend.md) — Vue 3 Frontend (API URL'lerini nasıl tüketiyor)
- [ADR-013](ADR-013-sonarqube.md) — SonarQube (API kalite kontrolü)
