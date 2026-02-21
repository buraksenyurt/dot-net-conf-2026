using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

public class GetVehicleOptionsByVehicleQueryHandler
    : IRequestHandler<GetVehicleOptionsByVehicleQuery, Result<IEnumerable<VehicleOptionDto>>>
{
    private readonly IVehicleOptionRepository _vehicleOptionRepository;

    public GetVehicleOptionsByVehicleQueryHandler(IVehicleOptionRepository vehicleOptionRepository)
    {
        _vehicleOptionRepository = vehicleOptionRepository;
    }

    public async Task<Result<IEnumerable<VehicleOptionDto>>> Handle(
        GetVehicleOptionsByVehicleQuery request,
        CancellationToken cancellationToken)
    {
        var options = await _vehicleOptionRepository.GetByVehicleIdAsync(request.VehicleId, cancellationToken);

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
            o.ServiceAdvisor?.GetDisplayName()));

        return Result<IEnumerable<VehicleOptionDto>>.Success(dtos);
    }
}
