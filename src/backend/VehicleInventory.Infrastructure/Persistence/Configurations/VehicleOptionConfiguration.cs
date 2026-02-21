using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Infrastructure.Persistence.Configurations;

public class VehicleOptionConfiguration : IEntityTypeConfiguration<VehicleOption>
{
    public void Configure(EntityTypeBuilder<VehicleOption> builder)
    {
        builder.ToTable("VehicleOptions");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.VehicleId).IsRequired();
        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.ExpiresAt).IsRequired();
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);

        builder.OwnsOne(o => o.OptionFee, money =>
        {
            money.Property(m => m.Amount).HasColumnName("OptionFeeAmount").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("OptionFeeCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasOne(o => o.Vehicle)
               .WithMany()
               .HasForeignKey(o => o.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Customer)
               .WithMany()
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.ServiceAdvisor)
               .WithMany()
               .HasForeignKey(o => o.ServiceAdvisorId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(o => o.VehicleId);
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.ServiceAdvisorId);
        builder.HasIndex(o => o.Status);
    }
}
