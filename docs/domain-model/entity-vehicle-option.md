# Entity: VehicleOption

## Genel Bakış

`VehicleOption`, bir müşterinin belirli bir aracı satın alma opsiyonu ile belirli bir süre için rezerve etmesini temsil eden domain entity'sidir. US-003 kapsamında tanımlanmıştır.

## Özellikler

| Alan              | Tip                  | Açıklama                                          |
|-------------------|----------------------|---------------------------------------------------|
| `Id`              | `Guid`               | Benzersiz tanımlayıcı                             |
| `VehicleId`       | `Guid`               | İlişkili araç (FK → Vehicle)                      |
| `CustomerId`      | `Guid`               | Opsiyon sahibi müşteri (FK → Customer)            |
| `ExpiresAt`       | `DateTime`           | Opsiyonun bitiş tarihi (UTC)                      |
| `OptionFee`       | `Money`              | Opsiyon/depozito ücreti                           |
| `Notes`           | `string?`            | Opsiyonel satış notu                              |
| `Status`          | `VehicleOptionStatus`| Opsiyonun anlık durumu                            |
| `CreatedAt`       | `DateTime`           | Oluşturulma tarihi (UTC)                          |
| `UpdatedAt`       | `DateTime?`          | Son güncelleme tarihi                             |

## İlişkiler

- `Vehicle` → Navigation property (bir opsiyonun bir aracı var)
- `Customer` → Navigation property (bir opsiyonun bir müşterisi var)

## Durum Makinesi (VehicleOptionStatus)

```
Active ──── Cancel() ──→ Cancelled
  │
  └── ExpiresAt geçince → Expired (sorgulama anında hesaplanır)
```

| Durum       | Değer | Açıklama                              |
|-------------|-------|---------------------------------------|
| `Active`    | 1     | Opsiyon geçerli                       |
| `Expired`   | 2     | Süre dolmuş, araç serbest             |
| `Cancelled` | 3     | İptal edilmiş, araç serbest           |

## İş Kuralları

1. **Oluşturma (Create)**:
   - Araç `InStock` veya `OnSale` statüsünde olmalı
   - Araç `Reserved` veya `Sold` ise opsiyon oluşturulamaz
   - Geçerlilik süresi 1–30 gün
   - OptionFee ≥ 0
   - Oluşturma anında araç `Reserved` statüsüne geçirilir

2. **İptal (Cancel)**:
   - Sadece `Active` opsiyonlar iptal edilebilir
   - İptal sonrası araç `OnSale` statüsüne döner

## Factory Method

```csharp
VehicleOption.Create(
    vehicle: Vehicle,
    customer: Customer,
    validityDays: int,       // 1-30
    optionFee: Money,        // Amount >= 0
    notes: string?
) → Result<VehicleOption>
```

## Value Objects

- `OptionFee` → `Money` value object (`Amount`, `Currency`)

## Veritabanı Tablosu: `VehicleOptions`

| Kolon             | Tip             | Kısıtlamalar                      |
|-------------------|-----------------|------------------------------------|
| `Id`              | `uuid`          | PK                                 |
| `VehicleId`       | `uuid`          | FK → Vehicles(Id), NOT NULL        |
| `CustomerId`      | `uuid`          | FK → Customers(Id), NOT NULL       |
| `ExpiresAt`       | `timestamp`     | NOT NULL                           |
| `OptionFeeAmount` | `decimal(18,2)` | NOT NULL                           |
| `OptionFeeCurrency`| `varchar(3)`   | NOT NULL                           |
| `Notes`           | `varchar(500)`  | NULL                               |
| `Status`          | `varchar(20)`   | NOT NULL                           |
| `CreatedAt`       | `timestamp`     | NOT NULL                           |
| `UpdatedAt`       | `timestamp`     | NULL                               |
