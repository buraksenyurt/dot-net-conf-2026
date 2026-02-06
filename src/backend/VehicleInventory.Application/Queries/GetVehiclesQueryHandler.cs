using MediatR;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Handler for GetVehiclesQuery.
/// </summary>
public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, PagedResult<VehicleDto>>
{
    private readonly IVehicleRepository _repository;

    public GetVehiclesQueryHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<VehicleDto>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var (vehicles, totalCount) = await _repository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.Brand,
            request.Status,
            cancellationToken
        );

        var vehicleDtos = vehicles.Select(v => new VehicleDto(
            v.Id,
            v.VIN.Value,
            v.Brand,
            v.Model,
            v.Year,
            v.EngineType.ToString(),
            v.Mileage,
            v.Color,
            v.PurchasePrice.Amount,
            v.PurchasePrice.Currency,
            v.SuggestedPrice.Amount,
            v.SuggestedPrice.Currency,
            v.TransmissionType.ToString(),
            v.FuelConsumption,
            v.EngineCapacity,
            v.Features,
            v.Status.ToString(),
            v.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PagedResult<VehicleDto>(
            vehicleDtos,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        );
    }
}
