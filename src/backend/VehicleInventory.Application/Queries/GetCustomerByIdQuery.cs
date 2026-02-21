using MediatR;
using VehicleInventory.Application.DTOs;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to retrieve a single customer by their unique identifier.
/// Returns null when the customer is not found.
/// </summary>
public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;
