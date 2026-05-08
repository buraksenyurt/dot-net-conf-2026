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

    public async Task<(IEnumerable<VehicleOption> Items, int TotalCount)> GetSummaryAsync(
        string? customerSearch,
        string? vehicleSearch,
        VehicleOptionStatus? status,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var query = _context.VehicleOptions
            .Include(o => o.Vehicle)
            .Include(o => o.Customer)
            .Include(o => o.ServiceAdvisor)
            .AsQueryable();

        // BR-5: case-insensitive partial match on customer FirstName/LastName
        if (!string.IsNullOrWhiteSpace(customerSearch))
        {
            var term = customerSearch.Trim().ToLower();
            query = query.Where(o =>
                o.Customer.FirstName.ToLower().Contains(term) ||
                o.Customer.LastName.ToLower().Contains(term));
        }

        // BR-5: case-insensitive partial match on Vehicle Brand/Model/VIN
        if (!string.IsNullOrWhiteSpace(vehicleSearch))
        {
            var term = vehicleSearch.Trim().ToLower();
            query = query.Where(o =>
                o.Vehicle.Brand.ToLower().Contains(term) ||
                o.Vehicle.Model.ToLower().Contains(term) ||
                o.Vehicle.VIN.Value.ToLower().Contains(term));
        }

        // BR-2: status filter accounts for runtime-computed Expired state
        if (status.HasValue)
        {
            query = status.Value switch
            {
                VehicleOptionStatus.Active =>
                    query.Where(o => o.Status == VehicleOptionStatus.Active && o.ExpiresAt >= utcNow),
                VehicleOptionStatus.Expired =>
                    query.Where(o =>
                        o.Status == VehicleOptionStatus.Expired ||
                        (o.Status == VehicleOptionStatus.Active && o.ExpiresAt < utcNow)),
                VehicleOptionStatus.Cancelled =>
                    query.Where(o => o.Status == VehicleOptionStatus.Cancelled),
                _ => query
            };
        }

        if (createdFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= createdFrom.Value);

        if (createdTo.HasValue)
            query = query.Where(o => o.CreatedAt <= createdTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        // BR-3: sorting
        var isAsc = !string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "createdat" => isAsc
                ? query.OrderBy(o => o.CreatedAt)
                : query.OrderByDescending(o => o.CreatedAt),
            "customername" => isAsc
                ? query.OrderBy(o => o.Customer.LastName).ThenBy(o => o.Customer.FirstName)
                : query.OrderByDescending(o => o.Customer.LastName).ThenByDescending(o => o.Customer.FirstName),
            _ => isAsc  // default: expiresAt
                ? query.OrderBy(o => o.ExpiresAt)
                : query.OrderByDescending(o => o.ExpiresAt)
        };

        // BR-4: pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
