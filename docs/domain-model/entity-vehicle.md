# Domain Model: Vehicle (Araç)

## Entity Tanımı

**Vehicle** - Envanterdeki fiziksel bir aracı temsil eden aggregate root entity.

## Sorumluluklar
- Araç bilgilerini saklamak ve yönetmek
- Araç durumunu (status) kontrol etmek
- Fiyat değişikliklerini yönetmek
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
- **EngineCapacity**: `int` - Motor hacmi (cc)
- **FuelConsumption**: `decimal` - Yakıt tüketimi (L/100km)
- **Mileage**: `int` - Kilometre (>= 0)

### Fiyat Bilgileri
- **PurchasePrice**: `Money (Value Object)` - Alış fiyatı
- **SuggestedPrice**: `Money (Value Object)` - Tavsiye edilen satış fiyatı
- **FinalPrice**: `Money (Value Object)` - Son satış fiyatı (nullable)

### Durum ve Takip
- **Status**: `VehicleStatus (Enum)` - Araç durumu
- **DealerId**: `Guid` - Hangi bayiye ait (Foreign Key)
- **Location**: `string` - Fiziksel konum (depo, showroom, vb.)
- **IsDisplayReady**: `bool` - Gösterim için hazır mı

### Donatım ve Özellikler
- **Features**: `List<string>` - Opsiyonel donatımlar

### Audit
- **CreatedAt**: `DateTime` - Oluşturulma tarihi
- **CreatedBy**: `string` - Oluşturan kullanıcı
- **UpdatedAt**: `DateTime?` - Güncellenme tarihi
- **UpdatedBy**: `string?` - Güncelleyen kullanıcı

## Domain Methods

### Constructor
```csharp
public Vehicle(
    VIN vin,
    string brand,
    string model,
    int year,
    string color,
    EngineType engineType,
    TransmissionType transmissionType,
    Money purchasePrice,
    Guid dealerId)
```

### Business Logic Methods

```csharp
// Durumu değiştir
public void UpdateStatus(VehicleStatus newStatus)
{
    // İş kuralı: Satıldı durumundan geri dönemez
    if (Status == VehicleStatus.Sold && newStatus != VehicleStatus.Sold)
        throw new VehicleDomainException("Satılmış araç durumu değiştirilemez");
    
    Status = newStatus;
    RaiseDomainEvent(new VehicleStatusChangedEvent(Id, newStatus));
}

// Fiyat güncelle
public void UpdatePrice(Money newSuggestedPrice)
{
    // İş kuralı: Tavsiye fiyat, alış fiyatından düşük olamaz
    if (newSuggestedPrice.Amount < PurchasePrice.Amount)
        throw new VehicleDomainException("Satış fiyatı alış fiyatından düşük olamaz");
    
    SuggestedPrice = newSuggestedPrice;
}

// Aracı sat
public void Sell(Money finalPrice, string soldBy)
{
    // İş kuralı: Sadece stokta veya rezerve olan araç satılabilir
    if (Status != VehicleStatus.Available && Status != VehicleStatus.Reserved)
        throw new VehicleDomainException("Bu durumdaki araç satılamaz");
    
    FinalPrice = finalPrice;
    Status = VehicleStatus.Sold;
    RaiseDomainEvent(new VehicleSoldEvent(Id, VIN, finalPrice, soldBy));
}

// Kilometre güncelle
public void UpdateMileage(int newMileage)
{
    // İş kuralı: Kilometre geriye gidemez
    if (newMileage < Mileage)
        throw new VehicleDomainException("Kilometre değeri geriye güncellenemez");
    
    Mileage = newMileage;
}

// Gösterim için hazırla
public void MarkAsDisplayReady()
{
    IsDisplayReady = true;
    if (Status == VehicleStatus.Available)
        Location = "Showroom";
}
```

## Domain Events

```csharp
public class VehicleAddedToInventoryEvent : DomainEvent
{
    public Guid VehicleId { get; }
    public VIN VIN { get; }
    public Guid DealerId { get; }
}

public class VehicleStatusChangedEvent : DomainEvent
{
    public Guid VehicleId { get; }
    public VehicleStatus NewStatus { get; }
}

public class VehicleSoldEvent : DomainEvent
{
    public Guid VehicleId { get; }
    public VIN VIN { get; }
    public Money FinalPrice { get; }
    public string SoldBy { get; }
}
```

## Validations (Domain Invariants)

```csharp
private void ValidateInvariants()
{
    if (string.IsNullOrWhiteSpace(Brand))
        throw new VehicleDomainException("Marka boş olamaz");
    
    if (string.IsNullOrWhiteSpace(Model))
        throw new VehicleDomainException("Model boş olamaz");
    
    if (Year < 1900 || Year > DateTime.Now.Year + 1)
        throw new VehicleDomainException("Geçersiz model yılı");
    
    if (Mileage < 0)
        throw new VehicleDomainException("Kilometre negatif olamaz");
    
    if (PurchasePrice.Amount <= 0)
        throw new VehicleDomainException("Alış fiyatı pozitif olmalı");
}
```

## Relationships

- **Many-to-One** with `Dealer` - Bir araç bir bayiye aittir
- **One-to-Many** with `ServiceHistory` - Bir aracın birden fazla servis kaydı olabilir
- **One-to-Many** with `TestDrive` - Bir araçta birden fazla test sürüşü yapılabilir

## Indexes

```csharp
// Database indexes
- PK: Id
- UNIQUE: VIN
- INDEX: DealerId, Status
- INDEX: Brand, Model
- INDEX: CreatedAt
```

## Aggregate Boundary

**Vehicle** kendi içinde tutarlı bir aggregate root'tur ve:
- Doğrudan `ServiceHistory` aggregate'ine referans vermez (sadece Id ile)
- `Dealer` bilgisini sadece Id ile tutar
- Transactional boundary olarak davranır

## Entity Framework Configuration Örneği

```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.VIN)
            .HasConversion(
                v => v.Value,
                v => new VIN(v))
            .HasMaxLength(17)
            .IsRequired();
        
        builder.HasIndex(v => v.VIN).IsUnique();
        
        builder.Property(v => v.Brand).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Model).HasMaxLength(100).IsRequired();
        
        builder.OwnsOne(v => v.PurchasePrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("PurchaseAmount");
            money.Property(m => m.Currency).HasColumnName("PurchaseCurrency");
        });
        
        // ... diğer konfigürasyonlar
    }
}
```

## Kullanım Örnekleri

```csharp
// Yeni araç oluşturma
var vin = new VIN("1HGBH41JXMN109186");
var purchasePrice = new Money(1500000, Currency.TRY);
var vehicle = new Vehicle(
    vin,
    "Honda",
    "Civic",
    2026,
    "Gümüş",
    EngineType.Hybrid,
    TransmissionType.Automatic,
    purchasePrice,
    dealerId
);

// Fiyat güncelleme
var newPrice = new Money(1725000, Currency.TRY);
vehicle.UpdatePrice(newPrice);

// Araç satışı
var finalPrice = new Money(1700000, Currency.TRY);
vehicle.Sell(finalPrice, "salesperson@email.com");
```
