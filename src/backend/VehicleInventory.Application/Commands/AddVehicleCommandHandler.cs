using MediatR;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Handler for AddVehicleCommand.
/// </summary>
public class AddVehicleCommandHandler : IRequestHandler<AddVehicleCommand, Result<Guid>>
{
    private readonly IVehicleRepository _repository;

    public AddVehicleCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        // Check if vehicle with same VIN already exists
        if (await _repository.ExistsAsync(request.Vin, cancellationToken))
        {
            return Result<Guid>.Failure($"Vehicle with VIN {request.Vin} already exists");
        }

        // Create VIN value object
        var vinResult = VIN.Create(request.Vin);
        if (vinResult.IsFailure)
        {
            return Result<Guid>.Failure(vinResult.Error);
        }

        // Create Money value objects
        var purchasePriceResult = Money.Create(request.PurchaseAmount, request.PurchaseCurrency);
        if (purchasePriceResult.IsFailure)
        {
            return Result<Guid>.Failure($"Purchase price error: {purchasePriceResult.Error}");
        }

        var suggestedPriceResult = Money.Create(request.SuggestedAmount, request.SuggestedCurrency);
        if (suggestedPriceResult.IsFailure)
        {
            return Result<Guid>.Failure($"Suggested price error: {suggestedPriceResult.Error}");
        }

        // Parse enums
        if (!Enum.TryParse<EngineType>(request.EngineType, ignoreCase: true, out var engineType))
        {
            return Result<Guid>.Failure($"Invalid engine type: {request.EngineType}");
        }

        if (!Enum.TryParse<TransmissionType>(request.TransmissionType, ignoreCase: true, out var transmissionType))
        {
            return Result<Guid>.Failure($"Invalid transmission type: {request.TransmissionType}");
        }

        // Create Vehicle entity
        var vehicleResult = Vehicle.Create(
            vinResult.Value!,
            request.Brand,
            request.Model,
            request.Year,
            engineType,
            request.Mileage,
            request.Color,
            purchasePriceResult.Value!,
            suggestedPriceResult.Value!,
            transmissionType,
            request.FuelConsumption,
            request.EngineCapacity,
            request.Features
        );

        if (vehicleResult.IsFailure)
        {
            return Result<Guid>.Failure(vehicleResult.Error);
        }

        // Save to repository
        var savedVehicle = await _repository.AddAsync(vehicleResult.Value!, cancellationToken);

        return Result<Guid>.Success(savedVehicle.Id);
    }
}
