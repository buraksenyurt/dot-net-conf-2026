using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Domain.Entities;

/// <summary>
/// Vehicle aggregate root representing a vehicle in the inventory.
/// Follows Domain-Driven Design principles.
/// </summary>
public sealed class Vehicle
{
    public Guid Id { get; private set; }
    public VIN VIN { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public EngineType EngineType { get; private set; }
    public int Mileage { get; private set; }
    public string Color { get; private set; }
    public Money PurchasePrice { get; private set; }
    public Money SuggestedPrice { get; private set; }
    public TransmissionType TransmissionType { get; private set; }
    public decimal FuelConsumption { get; private set; }
    public int EngineCapacity { get; private set; }
    public List<string> Features { get; private set; }
    public VehicleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private Vehicle()
    {
        VIN = null!;
        Brand = null!;
        Model = null!;
        Color = null!;
        PurchasePrice = null!;
        SuggestedPrice = null!;
        Features = new List<string>();
    }

    private Vehicle(
        Guid id,
        VIN vin,
        VehicleSpecification specification,
        Money purchasePrice,
        Money suggestedPrice)
    {
        Id = id;
        VIN = vin;
        Brand = specification.Brand.Trim();
        Model = specification.Model.Trim();
        Year = specification.Year;
        EngineType = specification.EngineType;
        Mileage = specification.Mileage;
        Color = specification.Color.Trim();
        PurchasePrice = purchasePrice;
        SuggestedPrice = suggestedPrice;
        TransmissionType = specification.TransmissionType;
        FuelConsumption = specification.FuelConsumption;
        EngineCapacity = specification.EngineCapacity;
        Features = specification.Features is null
            ? new List<string>()
            : new List<string>(specification.Features);
        Status = VehicleStatus.InStock;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new Vehicle with validation.
    /// </summary>
    public static Result<Vehicle> Create(
        VIN vin,
        VehicleSpecification specification,
        Money purchasePrice,
        Money suggestedPrice)
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(specification.Brand))
            return Result<Vehicle>.Failure("Brand is required");

        if (string.IsNullOrWhiteSpace(specification.Model))
            return Result<Vehicle>.Failure("Model is required");

        if (specification.Year > DateTime.UtcNow.Year)
            return Result<Vehicle>.Failure($"Year cannot be greater than {DateTime.UtcNow.Year}");

        if (specification.Year < 1900)
            return Result<Vehicle>.Failure("Year must be 1900 or later");

        if (specification.Mileage < 0)
            return Result<Vehicle>.Failure("Mileage cannot be negative");

        if (string.IsNullOrWhiteSpace(specification.Color))
            return Result<Vehicle>.Failure("Color is required");

        // Business Rule: Suggested price must be greater than or equal to purchase price
        if (purchasePrice.Currency != suggestedPrice.Currency)
            return Result<Vehicle>.Failure("Purchase price and suggested price must have the same currency");

        if (suggestedPrice.Amount < purchasePrice.Amount)
            return Result<Vehicle>.Failure("Suggested price cannot be less than purchase price");

        if (specification.FuelConsumption < 0)
            return Result<Vehicle>.Failure("Fuel consumption cannot be negative");

        // Engine capacity must be > 0 for non-electric vehicles
        if (specification.EngineType != EngineType.Electric && specification.EngineCapacity <= 0)
            return Result<Vehicle>.Failure("Engine capacity must be greater than zero for non-electric vehicles");

        var vehicle = new Vehicle(
            Guid.NewGuid(),
            vin,
            specification,
            purchasePrice,
            suggestedPrice
        );

        return Result<Vehicle>.Success(vehicle);
    }

    /// <summary>
    /// Changes the vehicle status.
    /// </summary>
    public Result<Vehicle> ChangeStatus(VehicleStatus newStatus)
    {
        // Business rules for status transitions
        if (Status == VehicleStatus.Sold && newStatus != VehicleStatus.Sold)
            return Result<Vehicle>.Failure("Cannot change status of a sold vehicle");

        Status = newStatus;
        return Result<Vehicle>.Success(this);
    }

    /// <summary>
    /// Updates the mileage of the vehicle.
    /// </summary>
    public Result<Vehicle> UpdateMileage(int newMileage)
    {
        if (newMileage < Mileage)
            return Result<Vehicle>.Failure("New mileage cannot be less than current mileage");

        Mileage = newMileage;
        return Result<Vehicle>.Success(this);
    }

    /// <summary>
    /// Gets the full display name of the vehicle.
    /// </summary>
    public string GetDisplayName() => $"{Brand} {Model} {Year}";
}
