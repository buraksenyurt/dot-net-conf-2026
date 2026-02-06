using Microsoft.EntityFrameworkCore;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;

namespace VehicleInventory.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleInventoryDbContext _context;

    public VehicleRepository(VehicleInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Vehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.FirstOrDefaultAsync(v => v.VIN.Value == vin.ToUpperInvariant(), cancellationToken);
    }

    public async Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetAllAsync(
        int page = 1, int pageSize = 10, string? brand = null, string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Vehicles.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(v => v.Brand.ToLower().Contains(brand.ToLower()));
            
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<VehicleStatus>(status, true, out var statusEnum))
            query = query.Where(v => v.Status == statusEnum);
            
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            
        return (items, totalCount);
    }

    public async Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task<bool> ExistsAsync(string vin, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.AnyAsync(v => v.VIN.Value == vin.ToUpperInvariant(), cancellationToken);
    }
}
