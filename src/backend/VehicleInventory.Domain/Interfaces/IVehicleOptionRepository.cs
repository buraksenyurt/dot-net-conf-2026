using VehicleInventory.Domain.Entities;

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
    Task UpdateAsync(VehicleOption option, CancellationToken cancellationToken = default);
}
