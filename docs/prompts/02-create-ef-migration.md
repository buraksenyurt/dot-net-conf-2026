# Prompt: Entity Framework Migration Oluşturma

## Amaç
Domain model değişiklikleri için Entity Framework Core migration oluşturma ve uygulama.

## Prompt Template

```markdown
# Domain Model Değişiklikleri
[Değişen entity ve value object'leri listele]

# Migration Adı
[Açıklayıcı migration adı - örn: AddVehicleInventoryTable]

# Gereksinimler
1. Migration class oluştur
2. Entity Configuration sınıfları oluştur/güncelle
3. DbContext'e DbSet ekle
4. Seed data varsa ekle
5. Index'leri tanımla
6. Foreign key ilişkilerini kur

# Entity Configurations
Fluent API kullanarak:
- Column name'ler (PascalCase)
- Data types ve precision
- Required/Optional
- Max length
- Default values
- Indexes (unique, composite)
- Relationships

# Örnek İstek

Vehicle entity için migration oluştur:

Entity: Vehicle
Properties:
- Id (Guid, PK)
- VIN (string, 17 karakter, unique, not null)
- Brand (string, 100 karakter, not null)
- Model (string, 100 karakter, not null)
- Year (int, not null)
- Color (string, 50 karakter, not null)
- EngineType (enum -> int, not null)
- TransmissionType (enum -> int, not null)
- PurchasePrice (Money - owned type: Amount decimal(18,2), Currency int)
- SuggestedPrice (Money - owned type: Amount decimal(18,2), Currency int)
- Mileage (int, default: 0)
- Status (enum -> int, not null, default: 1)
- DealerId (Guid, FK, not null, index)
- CreatedAt (DateTime, not null)
- UpdatedAt (DateTime, nullable)

Indexes:
- UNIQUE: VIN
- INDEX: DealerId, Status
- INDEX: Brand, Model
- INDEX: CreatedAt

Lütfen:
1. VehicleConfiguration.cs dosyasını oluştur
2. Migration dosyasını oluştur
3. DbContext'e DbSet<Vehicle> ekle
```

## Kullanım

### Copilot Chat'te
```markdown
@workspace Yukarıdaki domain model için EF Core migration ve configuration oluştur.
Context: #file:docs/domain-model/entity-vehicle.md
```

## Beklenen Çıktı Örneği

### VehicleConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        // Primary Key
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .ValueGeneratedNever(); // Guid'ler domain'de üretilir

        // VIN (Value Object)
        builder.Property(v => v.VIN)
            .HasConversion(
                v => v.Value,
                v => new VIN(v))
            .HasMaxLength(17)
            .IsRequired()
            .HasColumnName("VIN");

        builder.HasIndex(v => v.VIN)
            .IsUnique()
            .HasDatabaseName("IX_Vehicles_VIN");

        // Basic Properties
        builder.Property(v => v.Brand)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("Brand");

        builder.Property(v => v.Model)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("Model");

        builder.Property(v => v.Year)
            .IsRequired()
            .HasColumnName("Year");

        builder.Property(v => v.Color)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("Color");

        // Enums
        builder.Property(v => v.EngineType)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("EngineType");

        builder.Property(v => v.TransmissionType)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("TransmissionType");

        builder.Property(v => v.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(VehicleStatus.Available)
            .HasColumnName("Status");

        // Money Value Objects (Owned Types)
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

        builder.OwnsOne(v => v.FinalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("FinalAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("FinalCurrency")
                .HasConversion<int>();
        });

        // Other Properties
        builder.Property(v => v.Mileage)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("Mileage");

        builder.Property(v => v.EngineCapacity)
            .HasColumnName("EngineCapacity");

        builder.Property(v => v.FuelConsumption)
            .HasPrecision(4, 1)
            .HasColumnName("FuelConsumption");

        // Foreign Keys
        builder.Property(v => v.DealerId)
            .IsRequired()
            .HasColumnName("DealerId");

        builder.HasIndex(v => new { v.DealerId, v.Status })
            .HasDatabaseName("IX_Vehicles_DealerId_Status");

        // Additional Indexes
        builder.HasIndex(v => new { v.Brand, v.Model })
            .HasDatabaseName("IX_Vehicles_Brand_Model");

        builder.HasIndex(v => v.CreatedAt)
            .HasDatabaseName("IX_Vehicles_CreatedAt");

        // Audit Fields
        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("CreatedBy");

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(v => v.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("UpdatedBy");

        // Ignore Domain Events (not persisted)
        builder.Ignore(v => v.DomainEvents);
    }
}
```

### Migration File
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleManagement.Infrastructure.Persistence.Migrations;

public partial class AddVehicleInventoryTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                VIN = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Year = table.Column<int>(type: "integer", nullable: false),
                Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                EngineType = table.Column<int>(type: "integer", nullable: false),
                TransmissionType = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                PurchaseAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                PurchaseCurrency = table.Column<int>(type: "integer", nullable: false),
                SuggestedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                SuggestedCurrency = table.Column<int>(type: "integer", nullable: false),
                FinalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                FinalCurrency = table.Column<int>(type: "integer", nullable: true),
                Mileage = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                EngineCapacity = table.Column<int>(type: "integer", nullable: true),
                FuelConsumption = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_VIN",
            table: "Vehicles",
            column: "VIN",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_DealerId_Status",
            table: "Vehicles",
            columns: new[] { "DealerId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_Brand_Model",
            table: "Vehicles",
            columns: new[] { "Brand", "Model" });

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_CreatedAt",
            table: "Vehicles",
            column: "CreatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Vehicles");
    }
}
```

## Migration Komutları

```bash
# Migration oluştur
dotnet ef migrations add AddVehicleInventoryTable -p src/VehicleManagement.Infrastructure -s src/VehicleManagement.Api

# Migration uygula
dotnet ef database update -p src/VehicleManagement.Infrastructure -s src/VehicleManagement.Api

# SQL script üret (review için)
dotnet ef migrations script -p src/VehicleManagement.Infrastructure -s src/VehicleManagement.Api -o migration.sql
```

## Notlar
- Her migration sonrası SQL script'i gözden geçir
- Production'da migration otomatik uygulanmamalı
- Seed data için ayrı migration düşün
- Rollback planı hazırla
