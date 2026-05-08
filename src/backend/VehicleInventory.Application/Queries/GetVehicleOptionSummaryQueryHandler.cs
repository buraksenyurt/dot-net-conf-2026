using MediatR;
using Microsoft.Extensions.Logging;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Handler for <see cref="GetVehicleOptionSummaryQuery"/> — US-007.
/// Applies BR-2: Active options whose ExpiresAt is in the past are mapped to Expired
/// in the response without modifying the database row.
/// </summary>
public class GetVehicleOptionSummaryQueryHandler
    : IRequestHandler<GetVehicleOptionSummaryQuery, PagedResult<VehicleOptionSummaryDto>>
{
    private readonly IVehicleOptionRepository _repository;
    private readonly ILogger<GetVehicleOptionSummaryQueryHandler> _logger;

    public GetVehicleOptionSummaryQueryHandler(
        IVehicleOptionRepository repository,
        ILogger<GetVehicleOptionSummaryQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<VehicleOptionSummaryDto>> Handle(
        GetVehicleOptionSummaryQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching vehicle option summary. Page: {Page}, PageSize: {PageSize}, CustomerSearch: {CustomerSearch}, VehicleSearch: {VehicleSearch}, Status: {Status}",
            request.Page, request.PageSize, request.CustomerSearch, request.VehicleSearch, request.Status);

        var (options, totalCount) = await _repository.GetSummaryAsync(
            request.CustomerSearch,
            request.VehicleSearch,
            request.Status,
            request.CreatedFrom,
            request.CreatedTo,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        _logger.LogInformation(
            "Found {Count} vehicle options out of {TotalCount} total.",
            options.Count(), totalCount);

        var utcNow = DateTime.UtcNow;

        var dtos = options.Select(o =>
        {
            // BR-2: derive runtime expiry state without touching the DB record
            var isExpired = o.Status == VehicleOptionStatus.Active && utcNow > o.ExpiresAt;
            var effectiveStatus = isExpired ? VehicleOptionStatus.Expired : o.Status;

            return new VehicleOptionSummaryDto(
                o.Id,
                o.VehicleId,
                o.Vehicle.GetDisplayName(),
                o.Vehicle.VIN.Value,
                o.CustomerId,
                o.Customer.GetDisplayName(),
                o.ServiceAdvisorId,
                o.ServiceAdvisor?.GetDisplayName(),
                o.ExpiresAt,
                o.OptionFee.Amount,
                o.OptionFee.Currency,
                o.Notes,
                (int)effectiveStatus,
                isExpired,
                o.CreatedAt);
        });

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PagedResult<VehicleOptionSummaryDto>(dtos, request.Page, request.PageSize, totalCount, totalPages);
    }
}
