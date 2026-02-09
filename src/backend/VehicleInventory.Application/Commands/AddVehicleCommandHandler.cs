using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AddVehicleCommandHandler> _logger;

    public AddVehicleCommandHandler(IVehicleRepository repository, ILogger<AddVehicleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to add new vehicle. VIN: {Vin}, Brand: {Brand}, Model: {Model}", request.Vin, request.Brand, request.Model);

        // Check if vehicle with same VIN already exists
        if (await _repository.ExistsAsync(request.Vin, cancellationToken))
        {
            _logger.LogWarning("Vehicle with VIN {Vin} already exists", request.Vin);
            return Result<Guid>.Failure($"Vehicle with VIN {request.Vin} already exists");
        }

        // Create VIN value object
        var vinResult = VIN.Create(request.Vin);
        if (vinResult.IsFailure)
        {
            _logger.LogError("Invalid VIN provided: {Vin}. Error: {Error}", request.Vin, vinResult.Error);
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

        // Create Vehicle specification
        var specification = new VehicleSpecification(
            request.Brand,
            request.Model,
            request.Year,
            engineType,
            request.Mileage,
            request.Color,
            transmissionType,
            request.FuelConsumption,
            request.EngineCapacity,
            request.Features
        );

        // Create Vehicle entity
        var vehicleResult = Vehicle.Create(
            vinResult.Value!,
            specification,
            purchasePriceResult.Value!,
            suggestedPriceResult.Value!
        );

        if (vehicleResult.IsFailure)
        {
            _logger.LogError("Failed to create vehicle entity: {Error}", vehicleResult.Error);
            return Result<Guid>.Failure(vehicleResult.Error);
        }

        // Save to repository
        var savedVehicle = await _repository.AddAsync(vehicleResult.Value!, cancellationToken);
        
        _logger.LogInformation("Vehicle added successfully. ID: {Id}, VIN: {Vin}", savedVehicle.Id, savedVehicle.VIN.Value);

        return Result<Guid>.Success(savedVehicle.Id);
    }
}
