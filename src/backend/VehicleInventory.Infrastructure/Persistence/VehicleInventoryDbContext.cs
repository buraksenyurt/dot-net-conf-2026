using Microsoft.EntityFrameworkCore;
using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Infrastructure.Persistence;

public class VehicleInventoryDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<VehicleOption> VehicleOptions { get; set; } = null!;

    public VehicleInventoryDbContext(DbContextOptions<VehicleInventoryDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehicleInventoryDbContext).Assembly);
    }
}
