# Static Data: Enum Değerleri

## VehicleStatus (Araç Durumu)

```csharp
public enum VehicleStatus
{
    Available = 1,    // Stokta
    Reserved = 2,     // Rezerve
    Sold = 3,         // Satıldı
    InService = 4,    // Serviste
    InTransit = 5     // Transfer Aşamasında
}
```

### UI Gösterimi
```typescript
export const VEHICLE_STATUS_LABELS = {
  Available: { label: 'Stokta', color: 'green', icon: 'check-circle' },
  Reserved: { label: 'Rezerve', color: 'yellow', icon: 'clock' },
  Sold: { label: 'Satıldı', color: 'gray', icon: 'check' },
  InService: { label: 'Serviste', color: 'blue', icon: 'tool' },
  InTransit: { label: 'Transfer', color: 'purple', icon: 'truck' }
};
```

## EngineType (Motor Tipi)

```csharp
public enum EngineType
{
    Petrol = 1,     // Benzin
    Diesel = 2,     // Dizel
    Electric = 3,   // Elektrik
    Hybrid = 4,     // Hibrit
    PlugInHybrid = 5 // Şarj Edilebilir Hibrit
}
```

### UI Gösterimi
```typescript
export const ENGINE_TYPE_LABELS = {
  Petrol: { label: 'Benzin', icon: '⛽' },
  Diesel: { label: 'Dizel', icon: '🚗' },
  Electric: { label: 'Elektrik', icon: '🔋' },
  Hybrid: { label: 'Hibrit', icon: '♻️' },
  PlugInHybrid: { label: 'Şarj Edilebilir Hibrit', icon: '🔌' }
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

## Currency (Para Birimi)

```csharp
public enum Currency
{
    TRY = 1,  // Türk Lirası
    USD = 2,  // US Dollar
    EUR = 3,  // Euro
    GBP = 4   // British Pound
}
```

### UI Gösterimi
```typescript
export const CURRENCY_SYMBOLS = {
  TRY: '₺',
  USD: '$',
  EUR: '€',
  GBP: '£'
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

```csharp
public enum DealerType
{
    Authorized = 1,     // Yetkili Bayi
    SubDealer = 2,      // Alt Bayi
    ServicePoint = 3    // Servis Noktası
}
```

## UserRole (Kullanıcı Rolleri)

```csharp
public enum UserRole
{
    Admin = 1,              // Sistem Yöneticisi
    DealerManager = 2,      // Bayi Yöneticisi
    SalesConsultant = 3,    // Satış Danışmanı
    ServiceAdvisor = 4,     // Servis Danışmanı
    RegionManager = 5,      // Bölge Müdürü
    InventoryManager = 6    // Stok Yöneticisi
}
```

### Yetki Matrisi
```typescript
export const ROLE_PERMISSIONS = {
  Admin: ['*'],  // Tüm yetkiler
  DealerManager: ['vehicle.view', 'vehicle.add', 'vehicle.edit', 'vehicle.delete', 'report.view'],
  SalesConsultant: ['vehicle.view', 'vehicle.reserve'],
  ServiceAdvisor: ['vehicle.view', 'service.manage'],
  RegionManager: ['vehicle.view', 'report.view', 'dealer.view'],
  InventoryManager: ['vehicle.view', 'vehicle.add', 'vehicle.edit', 'inventory.manage']
};
```

## API Response Format

```json
GET /api/v1/static-data/enums?type=VehicleStatus

{
  "enumName": "VehicleStatus",
  "values": [
    { "id": 1, "name": "Available", "displayName": "Stokta" },
    { "id": 2, "name": "Reserved", "displayName": "Rezerve" },
    { "id": 3, "name": "Sold", "displayName": "Satıldı" },
    { "id": 4, "name": "InService", "displayName": "Serviste" },
    { "id": 5, "name": "InTransit", "displayName": "Transfer" }
  ]
}
```
