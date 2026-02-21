using Microsoft.EntityFrameworkCore;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;

namespace VehicleInventory.Infrastructure.Repositories;

public class ServiceAdvisorRepository : IServiceAdvisorRepository
{
    private readonly VehicleInventoryDbContext _context;

    public ServiceAdvisorRepository(VehicleInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceAdvisor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceAdvisors
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<ServiceAdvisor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceAdvisors
            .FirstOrDefaultAsync(
                a => a.Email.Value == email,
                cancellationToken);
    }

    public async Task<IEnumerable<ServiceAdvisor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ServiceAdvisors
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceAdvisor> AddAsync(ServiceAdvisor advisor, CancellationToken cancellationToken = default)
    {
        await _context.ServiceAdvisors.AddAsync(advisor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return advisor;
    }
}
