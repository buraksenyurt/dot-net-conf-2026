# Value Object: VIN (Vehicle Identification Number)

## Tanım

**VIN**, bir aracın benzersiz kimlik numarasını temsil eden immutable value object.

## Sorumluluklar
- VIN formatını validate etmek
- ISO 3779 standardına uygunluğu kontrol etmek
- Check digit doğrulaması yapmak
- Equality comparison sağlamak

## Properties

```csharp
public class VIN : ValueObject
{
    public string Value { get; private set; }
    
    private VIN() { } // EF Core için
    
    public VIN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VIN boş olamaz");
        
        value = value.ToUpperInvariant().Trim();
        
        if (!IsValid(value))
            throw new ArgumentException($"Geçersiz VIN formatı: {value}");
        
        Value = value;
    }
}
```

## Validation Rules

### Format Kuralları
- Tam olarak 17 karakter uzunluğunda olmalı
- Sadece alfanumerik karakterler (A-Z, 0-9)
- I, O, Q harfleri kullanılamaz (1, 0 ile karışıklığı önlemek için)
- Tüm karakterler büyük harf olmalı

### Check Digit Algoritması

```csharp
private static bool IsValid(string vin)
{
    if (vin.Length != 17)
        return false;
    
    // Yasaklı karakterler
    if (vin.Any(c => c == 'I' || c == 'O' || c == 'Q'))
        return false;
    
    // Alfanumerik kontrol
    if (!vin.All(c => char.IsLetterOrDigit(c)))
        return false;
    
    // Check digit doğrulaması (9. pozisyon)
    return ValidateCheckDigit(vin);
}

private static bool ValidateCheckDigit(string vin)
{
    var weights = new[] { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };
    var transliterationTable = GetTransliterationTable();
    
    int sum = 0;
    for (int i = 0; i < 17; i++)
    {
        char c = vin[i];
        int value = char.IsDigit(c) ? c - '0' : transliterationTable[c];
        sum += value * weights[i];
    }
    
    int checkDigit = sum % 11;
    char expectedCheckDigit = checkDigit == 10 ? 'X' : (char)('0' + checkDigit);
    
    return vin[8] == expectedCheckDigit;
}
```

## Value Object Equality

```csharp
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return Value;
}

public override string ToString() => Value;

public static implicit operator string(VIN vin) => vin.Value;
```

## VIN Decode (Opsiyonel Özellikler)

VIN'den bilgi çıkartma:

```csharp
public VINInfo Decode()
{
    return new VINInfo
    {
        WorldManufacturerIdentifier = Value.Substring(0, 3),  // İlk 3 karakter: Üretici
        VehicleDescriptor = Value.Substring(3, 5),            // 4-8 karakterler: Araç özellikleri
        CheckDigit = Value[8],                                // 9. karakter
        ModelYear = DecodeModelYear(Value[9]),                // 10. karakter
        PlantCode = Value[10],                                // 11. karakter
        SequentialNumber = Value.Substring(11, 6)             // 12-17 karakterler
    };
}

private static int DecodeModelYear(char yearCode)
{
    // A=2010, B=2011, ..., Y=2024 (I, O, Q, U, Z atlanan)
    // 1=2001, 2=2002, ..., 9=2009
    // Döngüsel olarak devam eder
    // Basitleştirilmiş versiyon:
    if (char.IsDigit(yearCode))
        return 2000 + (yearCode - '0');
    
    // Harf tabanlı yıl decode mantığı...
    return 2010; // placeholder
}
```

## Kullanım Örnekleri

```csharp
// Yeni VIN oluşturma
var vin = new VIN("1HGBH41JXMN109186");

// Geçersiz VIN
try
{
    var invalidVin = new VIN("INVALID123"); // Exception fırlatır
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}

// Equality comparison
var vin1 = new VIN("1HGBH41JXMN109186");
var vin2 = new VIN("1HGBH41JXMN109186");
var areEqual = vin1 == vin2; // true

// String conversion
string vinString = vin; // Implicit conversion
Console.WriteLine(vin.ToString());

// VIN decode
var vinInfo = vin.Decode();
Console.WriteLine($"Üretici: {vinInfo.WorldManufacturerIdentifier}");
Console.WriteLine($"Model Yılı: {vinInfo.ModelYear}");
```

## Entity Framework Configuration

```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.Property(v => v.VIN)
            .HasConversion(
                v => v.Value,              // VIN -> string
                v => new VIN(v)            // string -> VIN
            )
            .HasMaxLength(17)
            .IsRequired();
        
        builder.HasIndex(v => v.VIN)
            .IsUnique();
    }
}
```

## Test Örnekleri

```csharp
[Fact]
public void Valid_VIN_Should_Be_Created()
{
    // Arrange & Act
    var vin = new VIN("1HGBH41JXMN109186");
    
    // Assert
    vin.Value.Should().Be("1HGBH41JXMN109186");
}

[Theory]
[InlineData("SHORT")]           // Çok kısa
[InlineData("TOOLONGVIN12345678")] // Çok uzun
[InlineData("1HGBH41JXMN10918I")] // Yasaklı karakter I
[InlineData("1HGBH41JXMN10918O")] // Yasaklı karakter O
[InlineData("1HGBH41JXMN10918Q")] // Yasaklı karakter Q
public void Invalid_VIN_Should_Throw_Exception(string invalidVin)
{
    // Act & Assert
    Action act = () => new VIN(invalidVin);
    act.Should().Throw<ArgumentException>();
}

[Fact]
public void VINs_With_Same_Value_Should_Be_Equal()
{
    // Arrange
    var vin1 = new VIN("1HGBH41JXMN109186");
    var vin2 = new VIN("1HGBH41JXMN109186");
    
    // Assert
    (vin1 == vin2).Should().BeTrue();
    vin1.GetHashCode().Should().Be(vin2.GetHashCode());
}
```

## Gerçek Dünya Örnekleri

```
1HGBH41JXMN109186  - Honda Civic
1FTFW1ET5EFA12345  - Ford F-150
WBAPH5C55BE123456  - BMW 3 Series
JM1BL1S59B1234567  - Mazda 3
```
