using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to retrieve all options belonging to a specific customer.
/// </summary>
public record GetVehicleOptionsByCustomerQuery(Guid CustomerId) : IRequest<Result<IEnumerable<VehicleOptionDto>>>;
