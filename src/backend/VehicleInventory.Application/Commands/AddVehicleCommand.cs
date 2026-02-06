using MediatR;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command to add a new vehicle to the inventory.
/// </summary>
public record AddVehicleCommand(
    string Vin,
    string Brand,
    string Model,
    int Year,
    string EngineType,
    int Mileage,
    string Color,
    decimal PurchaseAmount,
    string PurchaseCurrency,
    decimal SuggestedAmount,
    string SuggestedCurrency,
    string TransmissionType,
    decimal FuelConsumption,
    int EngineCapacity,
    List<string>? Features = null
) : IRequest<Result<Guid>>;
