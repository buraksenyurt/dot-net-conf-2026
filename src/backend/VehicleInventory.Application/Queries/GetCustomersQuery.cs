using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to retrieve a paginated and filtered list of customers.
/// </summary>
public record GetCustomersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? CustomerType = null
) : IRequest<PagedResult<CustomerDto>>;
