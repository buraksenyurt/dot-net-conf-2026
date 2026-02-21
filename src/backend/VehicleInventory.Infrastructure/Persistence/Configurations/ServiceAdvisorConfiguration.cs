using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Infrastructure.Persistence.Configurations;

public class ServiceAdvisorConfiguration : IEntityTypeConfiguration<ServiceAdvisor>
{
    public void Configure(EntityTypeBuilder<ServiceAdvisor> builder)
    {
        builder.ToTable("ServiceAdvisors");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.LastName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Department).HasMaxLength(100).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.OwnsOne(a => a.Email, email =>
        {
            email.Property(e => e.Value)
                 .HasColumnName("Email")
                 .HasMaxLength(254)
                 .IsRequired();
            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.HasIndex(a => a.IsActive);
        builder.HasIndex(a => new { a.LastName, a.FirstName });
    }
}
