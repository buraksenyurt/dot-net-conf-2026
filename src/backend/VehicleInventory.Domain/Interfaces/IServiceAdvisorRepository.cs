using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Domain.Interfaces;

/// <summary>
/// Repository interface for ServiceAdvisor persistence operations.
/// </summary>
public interface IServiceAdvisorRepository
{
    Task<ServiceAdvisor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceAdvisor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceAdvisor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceAdvisor> AddAsync(ServiceAdvisor advisor, CancellationToken cancellationToken = default);
}
