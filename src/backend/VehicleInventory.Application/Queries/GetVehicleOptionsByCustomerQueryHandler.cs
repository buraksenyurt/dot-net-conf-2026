using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

public class GetVehicleOptionsByCustomerQueryHandler
    : IRequestHandler<GetVehicleOptionsByCustomerQuery, Result<IEnumerable<VehicleOptionDto>>>
{
    private readonly IVehicleOptionRepository _vehicleOptionRepository;

    public GetVehicleOptionsByCustomerQueryHandler(IVehicleOptionRepository vehicleOptionRepository)
    {
        _vehicleOptionRepository = vehicleOptionRepository;
    }

    public async Task<Result<IEnumerable<VehicleOptionDto>>> Handle(
        GetVehicleOptionsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var options = await _vehicleOptionRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

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
            o.UpdatedAt));

        return Result<IEnumerable<VehicleOptionDto>>.Success(dtos);
    }
}
