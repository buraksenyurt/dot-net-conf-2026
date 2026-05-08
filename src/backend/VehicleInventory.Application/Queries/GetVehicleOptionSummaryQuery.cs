using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Enums;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query to retrieve a filterable and pageable summary of all vehicle options — US-007.
/// </summary>
public record GetVehicleOptionSummaryQuery(
    string? CustomerSearch,
    string? VehicleSearch,
    VehicleOptionStatus? Status,
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "expiresAt",
    string SortDirection = "asc"
) : IRequest<PagedResult<VehicleOptionSummaryDto>>;
