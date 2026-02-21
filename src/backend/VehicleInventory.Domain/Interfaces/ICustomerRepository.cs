using VehicleInventory.Domain.Entities;

namespace VehicleInventory.Domain.Interfaces;

/// <summary>
/// Repository interface for the Customer aggregate.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>Adds a new customer to the repository.</summary>
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>Gets a customer by its unique identifier.</summary>
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a customer by their e-mail address.</summary>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Returns whether an e-mail is already registered.</summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all customers with optional filtering and pagination.
    /// </summary>
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? customerType = null,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing customer.</summary>
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
}
