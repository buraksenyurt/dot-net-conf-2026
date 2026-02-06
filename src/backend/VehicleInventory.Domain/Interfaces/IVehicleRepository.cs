using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Domain.Interfaces;

/// <summary>
/// Repository interface for Vehicle aggregate.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Adds a new vehicle to the repository.
    /// </summary>
    Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a vehicle by its unique identifier.
    /// </summary>
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a vehicle by its VIN.
    /// </summary>
    Task<Vehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all vehicles with optional filtering and pagination.
    /// </summary>
    Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? brand = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing vehicle.
    /// </summary>
    Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a vehicle with the given VIN exists.
    /// </summary>
    Task<bool> ExistsAsync(string vin, CancellationToken cancellationToken = default);
}
