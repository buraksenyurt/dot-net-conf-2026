using Microsoft.EntityFrameworkCore;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;

namespace VehicleInventory.Infrastructure.Repositories;

public class VehicleOptionRepository : IVehicleOptionRepository
{
    private readonly VehicleInventoryDbContext _context;

    public VehicleOptionRepository(VehicleInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleOption> AddAsync(VehicleOption option, CancellationToken cancellationToken = default)
    {
        await _context.VehicleOptions.AddAsync(option, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return option;
    }

    public async Task<VehicleOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<VehicleOption?> GetActiveByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(
                o => o.VehicleId == vehicleId && o.Status == VehicleOptionStatus.Active,
                cancellationToken);
    }

    public async Task<IEnumerable<VehicleOption>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .Include(o => o.ServiceAdvisor)
            .Where(o => o.VehicleId == vehicleId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VehicleOption>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .Include(o => o.ServiceAdvisor)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VehicleOption>> GetByServiceAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .Include(o => o.ServiceAdvisor)
            .Where(o => o.ServiceAdvisorId == advisorId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(VehicleOption option, CancellationToken cancellationToken = default)
    {
        _context.VehicleOptions.Update(option);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
