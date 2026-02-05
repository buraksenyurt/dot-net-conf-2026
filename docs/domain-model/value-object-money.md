# Value Object: Money (Para)

## Tanım

**Money**, bir para miktarını ve para birimini temsil eden immutable value object. Para hesaplamaları ve dönüşümleri için kullanılır.

## Sorumluluklar
- Para miktarı ve para birimini birlikte saklamak
- Para birimi tutarlılığını garanti etmek
- Matematiksel işlemleri type-safe şekilde sağlamak
- Farklı para birimleri arası işlemleri engellemek

## Properties

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }
    
    private Money() { } // EF Core için
    
    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Para miktarı negatif olamaz");
        
        Amount = amount;
        Currency = currency;
    }
}
```

## Currency Enum

```csharp
public enum Currency
{
    TRY = 1,  // Türk Lirası
    USD = 2,  // US Dollar
    EUR = 3,  // Euro
    GBP = 4   // British Pound
}
```

## Domain Methods

### Matematiksel İşlemler

```csharp
// Toplama
public Money Add(Money other)
{
    if (Currency != other.Currency)
        throw new InvalidOperationException(
            $"Farklı para birimleri toplanamaz: {Currency} ve {other.Currency}");
    
    return new Money(Amount + other.Amount, Currency);
}

// Çıkarma
public Money Subtract(Money other)
{
    if (Currency != other.Currency)
        throw new InvalidOperationException(
            $"Farklı para birimleri çıkartılamaz: {Currency} ve {other.Currency}");
    
    if (Amount < other.Amount)
        throw new InvalidOperationException("Sonuç negatif olamaz");
    
    return new Money(Amount - other.Amount, Currency);
}

// Çarpma (yüzde hesaplama için)
public Money Multiply(decimal multiplier)
{
    if (multiplier < 0)
        throw new ArgumentException("Çarpan negatif olamaz");
    
    return new Money(Amount * multiplier, Currency);
}

// Bölme
public Money Divide(decimal divisor)
{
    if (divisor <= 0)
        throw new ArgumentException("Bölen sıfır veya negatif olamaz");
    
    return new Money(Amount / divisor, Currency);
}

// Yüzde hesaplama
public Money CalculatePercentage(decimal percentage)
{
    return new Money(Amount * (percentage / 100m), Currency);
}
```

### Karşılaştırma

```csharp
// Büyüktür
public bool IsGreaterThan(Money other)
{
    if (Currency != other.Currency)
        throw new InvalidOperationException("Farklı para birimleri karşılaştırılamaz");
    
    return Amount > other.Amount;
}

// Küçüktür
public bool IsLessThan(Money other)
{
    if (Currency != other.Currency)
        throw new InvalidOperationException("Farklı para birimleri karşılaştırılamaz");
    
    return Amount < other.Amount;
}

// Operator overloading
public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);
public static bool operator <(Money left, Money right) => left.IsLessThan(right);
public static bool operator >=(Money left, Money right) => !left.IsLessThan(right);
public static bool operator <=(Money left, Money right) => !left.IsGreaterThan(right);

public static Money operator +(Money left, Money right) => left.Add(right);
public static Money operator -(Money left, Money right) => left.Subtract(right);
public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);
public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);
```

### Para Birimi Dönüşümü

```csharp
// Not: Gerçek uygulamada exchange rate external service'ten alınır
public Money ConvertTo(Currency targetCurrency, decimal exchangeRate)
{
    if (Currency == targetCurrency)
        return this;
    
    if (exchangeRate <= 0)
        throw new ArgumentException("Döviz kuru pozitif olmalı");
    
    var convertedAmount = Amount * exchangeRate;
    return new Money(convertedAmount, targetCurrency);
}
```

## Value Object Equality

```csharp
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return Amount;
    yield return Currency;
}
```

## Formatting

```csharp
public string ToString(string format = "N2")
{
    var currencySymbol = Currency switch
    {
        Currency.TRY => "₺",
        Currency.USD => "$",
        Currency.EUR => "€",
        Currency.GBP => "£",
        _ => Currency.ToString()
    };
    
    return $"{Amount.ToString(format)} {currencySymbol}";
}

public string ToCompactString()
{
    if (Amount >= 1_000_000)
        return $"{(Amount / 1_000_000):N1}M {Currency}";
    if (Amount >= 1_000)
        return $"{(Amount / 1_000):N1}K {Currency}";
    return ToString();
}
```

## Kullanım Örnekleri

```csharp
// Yeni Money oluşturma
var price = new Money(1_500_000, Currency.TRY);
var discount = new Money(150_000, Currency.TRY);

// Matematiksel işlemler
var finalPrice = price - discount;  // 1,350,000 TRY

// Yüzde hesaplama
var tax = price.CalculatePercentage(18);  // KDV %18
var priceWithTax = price + tax;

// Kar marjı hesaplama
var costPrice = new Money(1_000_000, Currency.TRY);
var sellingPrice = new Money(1_200_000, Currency.TRY);
var profit = sellingPrice - costPrice;  // 200,000 TRY

// %15 indirim
var discountedPrice = sellingPrice * 0.85m;

// Karşılaştırma
if (sellingPrice > costPrice)
{
    Console.WriteLine("Karlı satış");
}

// Formatlama
Console.WriteLine(price.ToString());          // 1,500,000.00 ₺
Console.WriteLine(price.ToCompactString());   // 1.5M TRY

// Farklı para birimleri (HATA!)
var usdPrice = new Money(50_000, Currency.USD);
try
{
    var invalid = price + usdPrice;  // Exception fırlatır
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
}

// Para birimi dönüşümü
decimal usdToTryRate = 30.5m;
var tryEquivalent = usdPrice.ConvertTo(Currency.TRY, usdToTryRate);
Console.WriteLine(tryEquivalent);  // 1,525,000.00 ₺
```

## Entity Framework Configuration

```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        // Owned type olarak configure et
        builder.OwnsOne(v => v.PurchasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PurchaseAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.Property(m => m.Currency)
                .HasColumnName("PurchaseCurrency")
                .HasConversion<int>()
                .IsRequired();
        });
        
        builder.OwnsOne(v => v.SuggestedPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("SuggestedAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.Property(m => m.Currency)
                .HasColumnName("SuggestedCurrency")
                .HasConversion<int>()
                .IsRequired();
        });
    }
}
```

## Test Örnekleri

```csharp
[Fact]
public void Can_Add_Money_With_Same_Currency()
{
    // Arrange
    var money1 = new Money(1000, Currency.TRY);
    var money2 = new Money(500, Currency.TRY);
    
    // Act
    var result = money1 + money2;
    
    // Assert
    result.Amount.Should().Be(1500);
    result.Currency.Should().Be(Currency.TRY);
}

[Fact]
public void Cannot_Add_Money_With_Different_Currencies()
{
    // Arrange
    var tryMoney = new Money(1000, Currency.TRY);
    var usdMoney = new Money(100, Currency.USD);
    
    // Act & Assert
    Action act = () => tryMoney.Add(usdMoney);
    act.Should().Throw<InvalidOperationException>()
        .WithMessage("*Farklı para birimleri*");
}

[Fact]
public void Can_Calculate_Percentage()
{
    // Arrange
    var money = new Money(1000, Currency.TRY);
    
    // Act
    var result = money.CalculatePercentage(15); // %15
    
    // Assert
    result.Amount.Should().Be(150);
    result.Currency.Should().Be(Currency.TRY);
}

[Fact]
public void Cannot_Create_Negative_Money()
{
    // Act & Assert
    Action act = () => new Money(-100, Currency.TRY);
    act.Should().Throw<ArgumentException>();
}
```

## İş Kuralı Örnekleri

```csharp
// Minimum kar marjı kontrolü
public bool HasMinimumProfitMargin(Money cost, Money selling, decimal minimumMarginPercent)
{
    var profit = selling - cost;
    var profitMargin = (profit.Amount / cost.Amount) * 100;
    return profitMargin >= minimumMarginPercent;
}

// Kampanya fiyatı hesaplama
public Money CalculateCampaignPrice(Money originalPrice, decimal discountPercent, decimal minProfitPercent, Money costPrice)
{
    var discountedPrice = originalPrice * (1 - discountPercent / 100);
    
    // Minimum kar marjını kontrol et
    if (!HasMinimumProfitMargin(costPrice, discountedPrice, minProfitPercent))
    {
        throw new InvalidOperationException("Kampanya fiyatı minimum kar marjını sağlamıyor");
    }
    
    return discountedPrice;
}
```
