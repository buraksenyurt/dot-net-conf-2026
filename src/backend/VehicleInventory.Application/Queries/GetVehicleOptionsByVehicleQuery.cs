using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to retrieve all options for a specific vehicle ordered by creation date descending.
/// </summary>
public record GetVehicleOptionsByVehicleQuery(Guid VehicleId) : IRequest<Result<IEnumerable<VehicleOptionDto>>>;
