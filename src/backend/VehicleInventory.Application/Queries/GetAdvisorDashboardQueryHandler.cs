using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Query: returns all vehicle options assigned to a specific service advisor.
/// Used by the advisor's personal dashboard.
/// </summary>
public record GetAdvisorDashboardQuery(
    Guid AdvisorId
) : IRequest<Result<IEnumerable<VehicleOptionDto>>>;

public class GetAdvisorDashboardQueryHandler
    : IRequestHandler<GetAdvisorDashboardQuery, Result<IEnumerable<VehicleOptionDto>>>
{
    private readonly IVehicleOptionRepository _vehicleOptionRepository;
    private readonly IServiceAdvisorRepository _advisorRepository;

    public GetAdvisorDashboardQueryHandler(
        IVehicleOptionRepository vehicleOptionRepository,
        IServiceAdvisorRepository advisorRepository)
    {
        _vehicleOptionRepository = vehicleOptionRepository;
        _advisorRepository = advisorRepository;
    }

    public async Task<Result<IEnumerable<VehicleOptionDto>>> Handle(
        GetAdvisorDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var advisor = await _advisorRepository.GetByIdAsync(request.AdvisorId, cancellationToken);
        if (advisor is null)
            return Result<IEnumerable<VehicleOptionDto>>.Failure("Service advisor not found");

        var options = await _vehicleOptionRepository.GetByServiceAdvisorIdAsync(
            request.AdvisorId, cancellationToken);

        var dtos = options.Select(o => new VehicleOptionDto(
            o.Id,
            o.VehicleId,
            o.Vehicle.GetDisplayName(),
            o.Vehicle.VIN.Value,
            o.CustomerId,
            o.Customer.GetDisplayName(),
            o.ExpiresAt,
            o.OptionFee.Amount,
            o.OptionFee.Currency,
            o.Notes,
            o.Status.ToString(),
            o.IsExpired(),
            o.CreatedAt,
            o.UpdatedAt,
            o.ServiceAdvisorId,
            advisor.GetDisplayName()));

        return Result<IEnumerable<VehicleOptionDto>>.Success(dtos);
    }
}
