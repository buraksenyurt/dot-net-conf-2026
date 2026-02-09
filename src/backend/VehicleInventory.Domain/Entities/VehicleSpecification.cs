using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Domain.Entities;

/// <summary>
/// Value object that encapsulates vehicle specifications.
/// Reduces parameter count in Vehicle entity constructor.
/// </summary>
public sealed class VehicleSpecification
{
    public string Brand { get; }
    public string Model { get; }
    public int Year { get; }
    public EngineType EngineType { get; }
    public int Mileage { get; }
    public string Color { get; }
    public TransmissionType TransmissionType { get; }
    public decimal FuelConsumption { get; }
    public int EngineCapacity { get; }
    public List<string> Features { get; }

    public VehicleSpecification(
        string brand,
        string model,
        int year,
        EngineType engineType,
        int mileage,
        string color,
        TransmissionType transmissionType,
        decimal fuelConsumption,
        int engineCapacity,
        List<string>? features = null)
    {
        Brand = brand;
        Model = model;
        Year = year;
        EngineType = engineType;
        Mileage = mileage;
        Color = color;
        TransmissionType = transmissionType;
        FuelConsumption = fuelConsumption;
        EngineCapacity = engineCapacity;
        Features = features ?? new List<string>();
    }
}
