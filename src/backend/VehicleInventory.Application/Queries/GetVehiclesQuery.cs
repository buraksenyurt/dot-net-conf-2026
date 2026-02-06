using MediatR;
using VehicleInventory.Application.DTOs;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to get all vehicles with optional filtering and pagination.
/// </summary>
public record GetVehiclesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Brand = null,
    string? Status = null
) : IRequest<PagedResult<VehicleDto>>;
