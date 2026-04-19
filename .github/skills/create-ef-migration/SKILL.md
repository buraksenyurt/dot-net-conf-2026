---
name: create-ef-migration
description: Entity Framework Core migration ve entity configuration oluşturma. Fluent API ile kolon tanımları, Money ve VIN gibi value object owned type'ları, unique ve composite index'ler ve seed data içerir. Domain model değişikliklerine göre EntityConfiguration.cs, migration dosyası ve DbContext güncellemesini üretir.
---

# Skill: Entity Framework Core Migration Oluşturma

## Ne Zaman Kullanılır
- Yeni bir entity veritabanına eklenecekse
- Mevcut entity'e yeni kolon/ilişki eklenecekse
- Value object owned type konfigürasyonu gerekiyorsa
- Veritabanı seed data eklenecekse

## Gerekli Context Dosyaları
- `docs/domain-model/entity-*.md` — Entity tanımı ve kolon detayları
- `docs/domain-model/value-object-*.md` — Value Object'lerin yapısı
- `docs/static-data/enums.md` — Enum tanımları

## Proje Yapısı

```
src/backend/VehicleInventory.Infrastructure/
└── EntityFrameworkCore/
    ├── VehicleInventoryDbContext.cs         # DbSet eklenecek
    └── Configurations/
        └── [EntityName]Configuration.cs    # Yeni konfigürasyon dosyası
```

## Adım Adım Süreç

### 1. EntityConfiguration.cs Oluştur

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Infrastructure.EntityFrameworkCore.Configurations;

public class [EntityName]Configuration : IEntityTypeConfiguration<[EntityName]>
{
    public void Configure(EntityTypeBuilder<[EntityName]> builder)
    {
        builder.ToTable("[TableName]");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever(); // Guid'ler domain'de oluşturulur

        // String Properties
        builder.Property(x => x.PropertyName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("PropertyName");

        // Enum Properties
        builder.Property(x => x.EnumProperty)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("EnumProperty");

        // Value Object: VIN örneği
        builder.Property(x => x.VIN)
            .HasConversion(
                v => v.Value,
                v => new VIN(v))
            .HasMaxLength(17)
            .IsRequired()
            .HasColumnName("VIN");

        // Value Object: Money (Owned Type) örneği
        builder.OwnsOne(x => x.PurchasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasColumnName("PurchaseAmount");
            money.Property(m => m.Currency)
                .HasConversion<int>()
                .IsRequired()
                .HasColumnName("PurchaseCurrency");
        });

        // Indexes
        builder.HasIndex(x => x.VIN)
            .IsUnique()
            .HasDatabaseName("IX_[TableName]_VIN");

        builder.HasIndex(x => new { x.Brand, x.Model })
            .HasDatabaseName("IX_[TableName]_Brand_Model");

        // Foreign Key İlişki
        builder.HasOne<[RelatedEntity]>()
            .WithMany()
            .HasForeignKey(x => x.RelatedEntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 2. DbContext'e DbSet Ekle

```csharp
// VehicleInventoryDbContext.cs içine ekle
public DbSet<[EntityName]> [EntityName]s => Set<[EntityName]>();
```

### 3. Migration Oluştur

```bash
# Infrastructure projesinin bulunduğu dizinde veya --project flag ile
dotnet ef migrations add [MigrationName] \
    --project src/backend/VehicleInventory.Infrastructure \
    --startup-project src/backend/VehicleInventory.API
```

### 4. Migration Dosyasını İncele

Oluşturulan migration dosyasında şunların doğru olduğunu kontrol et:
- Tablo ve kolon adları PascalCase
- Decimal precision'ları `decimal(18,2)` formatında
- Unique index'ler tanımlanmış
- Foreign key constraint'ler doğru

### 5. Seed Data (Gerekirse)

```csharp
// Configuration sınıfının içinde
builder.HasData(
    new [EntityName]
    {
        Id = new Guid("..."),
        // Properties
    }
);
```

## Konfigürasyon Kuralları
- Tüm string property'lerde `HasMaxLength` tanımla
- Enum'ları `HasConversion<int>()` ile integer'a çevir
- `decimal` tipler için `HasColumnType("decimal(18,2)")` kullan
- Value Object'leri `OwnsOne` ile owned type olarak tanımla
- Guid PK'lar için `ValueGeneratedNever()` kullan (domain oluşturur)
- Tablo adları çoğul (Vehicles, Customers)
- Index isimleri: `IX_TableName_ColumnName` formatı

## Kontrol Listesi
- [ ] `IEntityTypeConfiguration<>` implement edildi
- [ ] Tüm property'lerin `HasColumnName` tanımlanmış
- [ ] String property'lerde `HasMaxLength` var
- [ ] Owned type value object'ler `OwnsOne` ile tanımlandı
- [ ] Unique constraint'ler `HasIndex().IsUnique()` ile eklendi
- [ ] DbContext'e `DbSet<>` eklendi
- [ ] `dotnet ef migrations add` komutu çalıştırıldı
- [ ] Migration dosyası incelendi, doğrulandı
