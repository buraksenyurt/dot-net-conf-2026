# Value Object: Money (Para)

## Tanım

**Money**, bir para miktarını ve para birimini birlikte temsil eden immutable value object.

## Sorumluluklar
- Para miktarı ve para birimini birlikte saklamak
- Para birimi tutarlılığını garanti etmek
- Farklı para birimleri arası işlemleri engellemek

## Properties

| Alan | Tip | Açıklama |
|---|---|---|
| `Amount` | `decimal` | Para miktarı (>= 0) |
| `Currency` | `string` | ISO 4217 para birimi kodu (3 karakter) |

**Desteklenen para birimleri:** `TRY`, `USD`, `EUR`, `GBP`, `JPY`, `CHF`

## Factory Method

```csharp
// Result pattern ile oluşturma; constructor private'tır
public static Result<Money> Create(decimal amount, string currency) → Result<Money>
```

**Validasyonlar:**
- `amount` negatif olamaz
- `currency` 3 karakterli, büyük harf ISO 4217 kodu olmalı

## Domain Methods

```csharp
// Toplama — para birimleri eşleşmezse InvalidOperationException
public Money Add(Money other) → Money

// Çıkarma — para birimleri eşleşmezse InvalidOperationException
public Money Subtract(Money other) → Money
```

## Value Object Equality

```csharp
// Eşitlik: Amount ve Currency birlikte karşılaştırılır
public bool Equals(Money? other)
public static bool operator ==(Money? left, Money? right)
public static bool operator !=(Money? left, Money? right)
```

## Kullanım Örnekleri

```csharp
// Oluşturma
var purchaseResult = Money.Create(1_500_000m, "TRY");
var optionFee = Money.Create(0m, "TRY").Value;  // Ücretsiz opsiyon için sıfır geçerli

// Toplama
var total = purchaseResult.Value.Add(Money.Create(50_000m, "TRY").Value);

// Araç fiyat kuralı
if (suggestedPrice.Amount < purchasePrice.Amount)
    return Result<Vehicle>.Failure("Suggested price cannot be less than purchase price");
```
