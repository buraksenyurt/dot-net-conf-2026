using MediatR;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Commands;

public class CancelVehicleOptionCommandHandler : IRequestHandler<CancelVehicleOptionCommand, Result<bool>>
{
    private readonly IVehicleOptionRepository _vehicleOptionRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public CancelVehicleOptionCommandHandler(
        IVehicleOptionRepository vehicleOptionRepository,
        IVehicleRepository vehicleRepository)
    {
        _vehicleOptionRepository = vehicleOptionRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result<bool>> Handle(CancelVehicleOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _vehicleOptionRepository.GetByIdAsync(request.OptionId, cancellationToken);
        if (option is null)
            return Result<bool>.Failure($"Option with id '{request.OptionId}' not found");

        // Cancel option — also calls vehicle.ChangeStatus(OnSale) internally
        var cancelResult = option.Cancel();
        if (cancelResult.IsFailure)
            return Result<bool>.Failure(cancelResult.Error);

        // Persist vehicle status change
        await _vehicleRepository.UpdateAsync(option.Vehicle, cancellationToken);

        // Persist option status change
        await _vehicleOptionRepository.UpdateAsync(option, cancellationToken);

        return Result<bool>.Success(true);
    }
}
