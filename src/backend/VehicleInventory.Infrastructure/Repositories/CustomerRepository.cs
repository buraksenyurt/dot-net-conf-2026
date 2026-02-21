using Microsoft.EntityFrameworkCore;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;

namespace VehicleInventory.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly VehicleInventoryDbContext _context;

    public CustomerRepository(VehicleInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email.Value == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Customers.AnyAsync(c => c.Email.Value == normalized, cancellationToken);
    }

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? customerType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(c =>
                EF.Functions.ILike(c.FirstName, $"%{searchTerm}%") ||
                EF.Functions.ILike(c.LastName, $"%{searchTerm}%") ||
                EF.Functions.ILike(c.Email.Value, $"%{searchTerm}%") ||
                (c.CompanyName != null && EF.Functions.ILike(c.CompanyName, $"%{searchTerm}%")));

        if (!string.IsNullOrWhiteSpace(customerType) &&
            Enum.TryParse<CustomerType>(customerType, ignoreCase: true, out var typeEnum))
            query = query.Where(c => c.CustomerType == typeEnum);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }
}
