# ADR-011: Backend Test Altyapısı — xUnit + FluentAssertions + Moq + TestContainers

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy sistemde birim testi neredeyse yoktu; iş mantığı veri erişim katmanıyla iç içe geçtiğinden izole test yazmak çok zordu. Modernizasyon hedeflerinden biri **%80+ test coverage** sağlamaktı. Bunun için:

- Test yazımı hızlı ve okunabilir olmalıydı
- Domain ve Application katmanları veritabanı olmadan (in-memory / mock) test edilebilmeliydi
- Infrastructure katmanı gerçek veritabanıyla entegrasyon testine tabi tutulabilmeliydi
- SonarQube ile coverage raporlama entegre çalışmalıydı

## Değerlendirilen Seçenekler

### Test Framework
- **xUnit**: .NET ekosisteminin fiili standardı, paralel test desteği, theory/fact ayrımı, modern.
- **NUnit**: Olgun, geniş ekosistem. xUnit'e kıyasla daha az modern sözdizimi.
- **MSTest**: Microsoft'un yerleşik framework'ü. Daha az esnek.

### Assertion Library
- **FluentAssertions**: `result.Should().Be(42)` sözdizimi, okunabilir hata mesajları.
- **Shouldly**: Benzer yaklaşım, daha az yaygın.
- **xUnit Assert**: Yerleşik, basit ama mesajları az açıklayıcı.

### Mock Library
- **Moq**: .NET'te en yaygın mock kütüphanesi, güçlü setup/verify API'si.
- **NSubstitute**: Daha sade sözdizimi. Moq kadar yaygın değil.

### Entegrasyon Testi (Infrastructure)
- **TestContainers**: Docker üzerinde gerçek PostgreSQL instance ayağa kaldırarak test. Gerçekçi, CI/CD uyumlu.
- **InMemory Provider**: EF Core'un in-memory sağlayıcısı. Hızlı ama PostgreSQL spesifik davranışları test etmez.

## Karar

Seçilen kombinasyon:

| Amaç | Kütüphane |
|------|-----------|
| Test framework | **xUnit** |
| Assertion | **FluentAssertions** |
| Mocking | **Moq** |
| Entegrasyon testi | **TestContainers** (PostgreSQL) |

Proje yapısı:
```
tests/
  VehicleInventory.Domain.Tests/       → Value Object ve Entity birim testleri
  VehicleInventory.Application.Tests/  → Command/Query/Validator birim testleri
```

Domain testleri örneği:
```csharp
[Fact]
public void VIN_ile_17_karakter_geçerli_olmalı()
{
    var vin = VIN.Create("1HGBH41JXMN109186");
    vin.Should().NotBeNull();
    vin.Value.Should().Be("1HGBH41JXMN109186");
}

[Fact]
public void Farklı_para_birimleri_toplanamaz()
{
    var tl  = Money.Of(100, "TRY");
    var usd = Money.Of(50, "USD");
    Action act = () => tl.Add(usd);
    act.Should().Throw<DomainException>();
}
```

Bu tercih yapıldı çünkü:
- xUnit + FluentAssertions kombinasyonu en yaygın, dokümantasyonu en zengin seçenek
- Moq, MediatR handler testlerinde repository mock'ları için ideal
- TestContainers ile CI pipeline'da gerçek PostgreSQL kullanılarak güven artar
- SonarQube, xUnit coverage raporlarını doğrudan okuyabilir

## Sonuçlar

### Olumlu Sonuçlar

- Domain ve Application katmanları veritabanı bağımlılığı olmadan hızlı test edilir
- `[Theory]` ile parametre tabanlı testler; tek bir test metodu birçok senaryoyu kapsar
- FluentAssertions hata mesajları CI log'larında kolayca okunur
- TestContainers ile entegrasyon testleri CI'da Docker üzerinde güvenilir şekilde çalışır

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- TestContainers Docker gerektirdiğinden Docker olmayan ortamlarda entegrasyon testleri çalışmaz
- Moq kurulumu bazen verbose; çok sayıda mock setup kodunu uzatır
- %80 coverage hedefi için sürekli izleme (SonarQube) ve discipline gerektirir

## İlgili ADR'ler

- [ADR-003](ADR-003-domain-driven-design.md) — DDD (Domain birim testlerinin kapsamı)
- [ADR-012](ADR-012-github-actions-cicd.md) — GitHub Actions (testlerin CI pipeline'a entegrasyonu)
- [ADR-013](ADR-013-sonarqube.md) — SonarQube (coverage raporlama)
