using MediatR;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command to create a new customer.
/// For individual customers, CompanyName and TaxNumber must be null.
/// For corporate customers, both CompanyName and TaxNumber are required.
/// </summary>
public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CustomerType,
    string? CompanyName = null,
    string? TaxNumber = null
) : IRequest<Result<Guid>>;
