# Static Data: Enum Değerleri

## VehicleStatus (Araç Durumu)

```csharp
public enum VehicleStatus
{
    InStock   = 1,  // Stokta (yeni eklenen araç)
    OnSale    = 2,  // Satışa açık
    Sold      = 3,  // Satıldı
    Reserved  = 4   // Rezerve (aktif opsiyon var)
}
```

### UI Gösterimi
```typescript
export const VEHICLE_STATUS_LABELS = {
  InStock:  { label: 'Stokta',   color: 'secondary', icon: 'box-seam' },
  OnSale:   { label: 'Satışta',  color: 'success',   icon: 'tag' },
  Sold:     { label: 'Satıldı',  color: 'dark',      icon: 'check-circle' },
  Reserved: { label: 'Rezerve',  color: 'warning',   icon: 'clock' }
};
```

## EngineType (Motor Tipi)

```csharp
public enum EngineType
{
    Gasoline = 1,  // Benzin
    Diesel   = 2,  // Dizel
    Electric = 3,  // Elektrik
    Hybrid   = 4   // Hibrit
}
```

### UI Gösterimi
```typescript
export const ENGINE_TYPE_LABELS = {
  Gasoline: { label: 'Benzin',   icon: '⛽' },
  Diesel:   { label: 'Dizel',    icon: '🚗' },
  Electric: { label: 'Elektrik', icon: '🔋' },
  Hybrid:   { label: 'Hibrit',   icon: '♻️' }
};
```

## TransmissionType (Vites Tipi)

```csharp
public enum TransmissionType
{
    Manual = 1,         // Manuel
    Automatic = 2,      // Otomatik
    SemiAutomatic = 3   // Yarı Otomatik
}
```

### UI Gösterimi
```typescript
export const TRANSMISSION_TYPE_LABELS = {
  Manual: { label: 'Manuel', shortLabel: 'M' },
  Automatic: { label: 'Otomatik', shortLabel: 'A' },
  SemiAutomatic: { label: 'Yarı Otomatik', shortLabel: 'SA' }
};
```

## CustomerType (Müşteri Tipi)

```csharp
public enum CustomerType
{
    Individual = 1,  // Bireysel
    Corporate  = 2   // Kurumsal
}
```

### UI Gösterimi
```typescript
export const CUSTOMER_TYPE_LABELS = {
  Individual: { label: 'Bireysel', icon: 'person' },
  Corporate:  { label: 'Kurumsal', icon: 'building' }
};
```

## VehicleOptionStatus (Opsiyon Durumu)

```csharp
public enum VehicleOptionStatus
{
    Active    = 1,  // Aktif (geçerli)
    Expired   = 2,  // Süresi dolmuş
    Cancelled = 3   // İptal edilmiş
}
```

### UI Gösterimi
```typescript
export const VEHICLE_OPTION_STATUS_LABELS = {
  Active:    { label: 'Aktif',     color: 'success', icon: 'check-circle' },
  Expired:   { label: 'Süresi Doldu', color: 'secondary', icon: 'calendar-x' },
  Cancelled: { label: 'İptal',     color: 'danger',  icon: 'x-circle' }
};
```

## VehicleColor (Araç Renkleri)

```typescript
export const VEHICLE_COLORS = [
  { value: 'white', label: 'Beyaz', hex: '#FFFFFF' },
  { value: 'black', label: 'Siyah', hex: '#000000' },
  { value: 'gray', label: 'Gri', hex: '#808080' },
  { value: 'silver', label: 'Gümüş', hex: '#C0C0C0' },
  { value: 'red', label: 'Kırmızı', hex: '#FF0000' },
  { value: 'blue', label: 'Mavi', hex: '#0000FF' },
  { value: 'green', label: 'Yeşil', hex: '#00FF00' },
  { value: 'yellow', label: 'Sarı', hex: '#FFFF00' },
  { value: 'brown', label: 'Kahverengi', hex: '#8B4513' },
  { value: 'orange', label: 'Turuncu', hex: '#FFA500' }
] as const;
```

## VehicleFeatures (Araç Donatımları)

```typescript
export const VEHICLE_FEATURES = [
  { value: 'Sunroof', label: 'Sunroof', category: 'comfort' },
  { value: 'LeatherSeats', label: 'Deri Döşeme', category: 'comfort' },
  { value: 'NavigationSystem', label: 'Navigasyon Sistemi', category: 'technology' },
  { value: 'ParkingSensors', label: 'Park Sensörleri', category: 'safety' },
  { value: 'Bluetooth', label: 'Bluetooth', category: 'technology' },
  { value: 'CruiseControl', label: 'Hız Sabitleyici', category: 'comfort' },
  { value: 'AlloyWheels', label: 'Alüminyum Jant', category: 'exterior' },
  { value: 'RearCamera', label: 'Geri Görüş Kamerası', category: 'safety' },
  { value: 'HeatedSeats', label: 'Koltuk Isıtma', category: 'comfort' },
  { value: 'AutomaticClimate', label: 'Otomatik Klima', category: 'comfort' },
  { value: 'LaneAssist', label: 'Şerit Takip Asistanı', category: 'safety' },
  { value: 'BlindSpotMonitor', label: 'Kör Nokta İkaz Sistemi', category: 'safety' },
  { value: 'AdaptiveCruise', label: 'Adaptif Hız Sabitleyici', category: 'technology' },
  { value: 'Xenon', label: 'Xenon Farlar', category: 'exterior' },
  { value: 'LED', label: 'LED Farlar', category: 'exterior' }
] as const;

export const FEATURE_CATEGORIES = {
  comfort: { label: 'Konfor', icon: 'sofa' },
  technology: { label: 'Teknoloji', icon: 'laptop' },
  safety: { label: 'Güvenlik', icon: 'shield' },
  exterior: { label: 'Dış Görünüm', icon: 'car' }
};
```

## DealerType (Bayi Tipi)

> ⚠️ Bu enum mevcut PoC kapsamında henüz implemente edilmemiştir.

## UserRole (Kullanıcı Rolleri)

> ⚠️ Bu enum mevcut PoC kapsamında henüz implemente edilmemiştir. Kimlik doğrulama için Keycloak entegrasyonu planlanmaktadır.

## Para Birimi (Currency)

`Money` value object'te para birimi bir enum değil, **ISO 4217 string kodu** olarak tutulur.

| Kod | Para Birimi |
|-----|-------------|
| `TRY` | Türk Lirası |
| `USD` | US Dollar |
| `EUR` | Euro |
| `GBP` | British Pound |
| `JPY` | Japon Yeni |
| `CHF` | İsviçre Frangı |
