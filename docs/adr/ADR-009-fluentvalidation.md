# ADR-009: Girdi Doğrulama — FluentValidation

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Uygulamada girdi doğrulaması birden fazla katmanda yapılması gerekiyordu:

- **API katmanı**: HTTP isteğiyle gelen DTO'ların formatı doğrulanmalı (zorunlu alanlar, uzunluk, format)
- **Application katmanı**: Command'ların iş kurallarına uygunluğu doğrulanmalı
- **Domain katmanı**: Value Object'ler (VIN, Email, Money) kendi invariant'larını korur

Legacy sistemde doğrulama kodu servisler arasında dağılmış, tutarsız ve test edilmesi zordu. Yeni yaklaşımda:
- Doğrulama kuralları merkezi ve okunabilir olmalı
- Hata mesajları çok dilli / özelleştirilebilir olmalı
- Test edilebilirlik yüksek olmalı
- MediatR pipeline'a entegre olabilmeli

## Değerlendirilen Seçenekler

- **Seçenek A — DataAnnotations (yerleşik)**: `[Required]`, `[MaxLength]`, `[EmailAddress]` attribute'ları. Basit ama karmaşık kurallar için yetersiz; test etmek zahmetli.
- **Seçenek B — FluentValidation**: Fluent API ile okunabilir doğrulama kuralları, zengin kural seti, MediatR pipeline entegrasyonu, kolay unit test.
- **Seçenek C — Manuel Doğrulama**: Handler içinde if-else blokları. Tam kontrol ama dağınık, tekrarlı ve test edilmesi zor.

## Karar

**FluentValidation** seçildi. **MediatR pipeline behavior** aracılığıyla Application katmanında otomatik çalışır.

Örnek validator:
```csharp
public class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.VIN)
            .NotEmpty().WithMessage("VIN zorunludur.")
            .Length(17).WithMessage("VIN tam 17 karakter olmalıdır.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");

        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("Marka seçimi zorunludur.");
    }
}
```

MediatR pipeline behavior (`ValidationBehavior<TRequest, TResponse>`):
- Handler çalışmadan önce ilgili validator'ı çağırır
- Doğrulama hatası varsa `ValidationException` fırlatır; handler hiç çalışmaz
- Global exception filter `ValidationException`'ı `400 Bad Request` olarak döndürür

Bu tercih yapıldı çünkü:
- Kurallar merkezi, okunabilir ve domain uzmanlarıyla paylaşılabilir
- Her validator için izole birim testi yazılabilir
- `WithMessage()` ile kullanıcıya dönük hata mesajları özelleştirilir
- `RuleSet` ile koşullu doğrulama senaryoları desteklenir

## Sonuçlar

### Olumlu Sonuçlar

- Command'lar handler'a ulaşmadan doğrulanır; handler kodu iş mantığına odaklanır
- Tüm doğrulama hataları tek API yanıtında toplanır (teker teker değil)
- Validator'lar injection ile test edilir; doğrulama senaryoları için ayrı test sınıfı yazılır
- Kurallar değiştirilmesi gerektiğinde tek bir dosyada yapılır

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Her Command ve Query için ayrı Validator sınıfı oluşturulması gerekebilir (küçük de olsa ek dosya)
- Domain katmanındaki Value Object doğrulaması ile Application katmanındaki FluentValidation arasında çift doğrulama olabilir; bilinçli kabul edilmiş ödünleşim (katman bağımsızlığı için değerli)

## İlgili ADR'ler

- [ADR-002](ADR-002-cqrs-mediatr.md) — CQRS ve MediatR (pipeline behavior entegrasyonu)
- [ADR-003](ADR-003-domain-driven-design.md) — DDD (Value Object invariant'ları ayrı bir doğrulama katmanı oluşturur)
