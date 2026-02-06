using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);
        
        builder.OwnsOne(v => v.VIN, vin => {
            vin.Property(v => v.Value).HasColumnName("VIN").HasMaxLength(17).IsRequired();
            vin.HasIndex(v => v.Value).IsUnique();
        });
        
        builder.OwnsOne(v => v.PurchasePrice, money => {
            money.Property(m => m.Amount).HasColumnName("PurchaseAmount").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("PurchaseCurrency").HasMaxLength(3);
        });
        
        builder.OwnsOne(v => v.SuggestedPrice, money => {
            money.Property(m => m.Amount).HasColumnName("SuggestedAmount").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("SuggestedCurrency").HasMaxLength(3);
        });
        
        builder.Property(v => v.Brand).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Model).HasMaxLength(100).IsRequired();
        builder.Property(v => v.EngineType).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.TransmissionType).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.Features).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
        );
        
        builder.HasIndex(v => v.Brand);
        builder.HasIndex(v => v.Status);
    }
}
