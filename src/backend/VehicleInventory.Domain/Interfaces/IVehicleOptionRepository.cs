using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;

namespace VehicleInventory.Domain.Interfaces;

/// <summary>
/// Repository interface for VehicleOption persistence operations.
/// </summary>
public interface IVehicleOptionRepository
{
    Task<VehicleOption> AddAsync(VehicleOption option, CancellationToken cancellationToken = default);
    Task<VehicleOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VehicleOption?> GetActiveByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<VehicleOption>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<VehicleOption>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<VehicleOption>> GetByServiceAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default);
    Task UpdateAsync(VehicleOption option, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filterable, sortable, paginated summary of all vehicle options — US-007.
    /// Status filtering accounts for the runtime Expired state (BR-2):
    /// when <paramref name="status"/> is <see cref="VehicleOptionStatus.Expired"/>,
    /// the query includes Active records whose ExpiresAt has already passed.
    /// </summary>
    Task<(IEnumerable<VehicleOption> Items, int TotalCount)> GetSummaryAsync(
        string? customerSearch,
        string? vehicleSearch,
        VehicleOptionStatus? status,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default);
}
