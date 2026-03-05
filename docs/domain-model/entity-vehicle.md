# Domain Model: Vehicle (Araç)

## Entity Tanımı

**Vehicle** - Envanterdeki fiziksel bir aracı temsil eden aggregate root entity.

## Sorumluluklar
- Araç bilgilerini saklamak ve yönetmek
- Araç durumunu (status) kontrol etmek
- İş kurallarını enforce etmek

## Properties

### Identity
- **Id**: `Guid` - Unique identifier (Primary Key)
- **VIN**: `VIN (Value Object)` - Vehicle Identification Number (Unique, Natural Key)

### Temel Bilgiler
- **Brand**: `string` - Marka (örn: Honda, Toyota)
- **Model**: `string` - Model (örn: Civic, Corolla)
- **Year**: `int` - Model yılı (1900 - CurrentYear)
- **Color**: `string` - Renk

### Teknik Özellikler
- **EngineType**: `EngineType (Enum)` - Motor tipi
- **TransmissionType**: `TransmissionType (Enum)` - Vites tipi
- **EngineCapacity**: `int` - Motor hacmi (cc); elektrikli araçlarda 0 olabilir
- **FuelConsumption**: `decimal` - Yakıt tüketimi (L/100km); elektrikli araçlarda 0 olabilir
- **Mileage**: `int` - Kilometre (>= 0)

### Fiyat Bilgileri
- **PurchasePrice**: `Money (Value Object)` - Alış fiyatı
- **SuggestedPrice**: `Money (Value Object)` - Tavsiye edilen satış fiyatı (>= PurchasePrice, aynı para birimi)

### Durum ve Takip
- **Status**: `VehicleStatus (Enum)` - Araç durumu (varsayılan: `InStock`)

### Donatım ve Özellikler
- **Features**: `List<string>` - Opsiyonel donatımlar

### Audit
- **CreatedAt**: `DateTime` - Oluşturulma tarihi (UTC)

## Factory Method

```csharp
// VehicleSpecification parametre nesnesini kullanır
public static Result<Vehicle> Create(
    VIN vin,
    VehicleSpecification specification,
    Money purchasePrice,
    Money suggestedPrice) → Result<Vehicle>
```

`VehicleSpecification` alanları: `Brand`, `Model`, `Year`, `EngineType`, `Mileage`, `Color`, `TransmissionType`, `FuelConsumption`, `EngineCapacity`, `Features`.

### Business Logic Methods

```csharp
// Durumu değiştir (Satılmış araç geri alınamaz)
public Result<Vehicle> ChangeStatus(VehicleStatus newStatus)

// Kilometre güncelle (geriye gidemez)
public Result<Vehicle> UpdateMileage(int newMileage)

// Görünen adı döner
public string GetDisplayName() // "Brand Model Year"
```

## İş Kuralları (Domain Invariants)

| Kural | Açıklama |
|---|---|
| `Brand` & `Model` | Boş olamaz |
| `Year` | 1900 ile mevcut yıl arasında olmalı |
| `Mileage` | Negatif olamaz |
| `PurchasePrice.Amount` | Pozitif olmalı |
| `SuggestedPrice >= PurchasePrice` | Aynı para biriminde olmalı |
| `FuelConsumption` | Negatif olamaz |
| `EngineCapacity` | Elektrikli olmayan araçlarda > 0 olmalı |
| `ChangeStatus` | Satılmış araç başka duruma alınamaz |
| `UpdateMileage` | Yeni kilometre mevcut değerden küçük olamaz |

## Relationships

- **One-to-Many** with `VehicleOption` — Bir araç için birden fazla opsiyon geçmişi olabilir, aynı anda yalnızca bir `Active` opsiyon bulunabilir

## Indexes

```
- PK: Id
- UNIQUE: VIN
- INDEX: Status
- INDEX: Brand, Model
- INDEX: CreatedAt
```

## Entity Framework Configuration Örneği

```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.VIN)
            .HasConversion(v => v.Value, v => VIN.Create(v).Value)
            .HasMaxLength(17)
            .IsRequired();
        builder.HasIndex(v => v.VIN).IsUnique();

        builder.Property(v => v.Brand).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Model).HasMaxLength(100).IsRequired();

        builder.OwnsOne(v => v.PurchasePrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("PurchaseAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("PurchaseCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(v => v.SuggestedPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SuggestedAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("SuggestedCurrency").HasMaxLength(3);
        });

        builder.Property(v => v.Status).HasConversion<int>();
        builder.Property(v => v.EngineType).HasConversion<int>();
        builder.Property(v => v.TransmissionType).HasConversion<int>();

        builder.Property(v => v.Features)
            .HasConversion(
                f => string.Join(',', f),
                f => f.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}
```

## Kullanım Örnekleri

```csharp
// VIN ve Money oluşturma
var vinResult = VIN.Create("1HGBH41JXMN109186");
var purchasePrice = Money.Create(1_500_000m, "TRY").Value;
var suggestedPrice = Money.Create(1_725_000m, "TRY").Value;

// Araç oluşturma
var spec = new VehicleSpecification(
    Brand: "Honda", Model: "Civic", Year: 2026, Color: "Gümüş",
    EngineType: EngineType.Hybrid, TransmissionType: TransmissionType.Automatic,
    Mileage: 0, FuelConsumption: 5.2m, EngineCapacity: 1500,
    Features: ["Sunroof", "LeatherSeats"]);

var result = Vehicle.Create(vinResult.Value, spec, purchasePrice, suggestedPrice);

// Durum değiştirme
result.Value.ChangeStatus(VehicleStatus.OnSale);
```
