namespace VehicleInventory.Domain.Enums;

/// <summary>
/// Represents the type of engine in a vehicle.
/// </summary>
public enum EngineType
{
    /// <summary>
    /// Gasoline/Petrol engine
    /// </summary>
    Gasoline = 1,

    /// <summary>
    /// Diesel engine
    /// </summary>
    Diesel = 2,

    /// <summary>
    /// Electric motor
    /// </summary>
    Electric = 3,

    /// <summary>
    /// Hybrid (combines gasoline/diesel with electric)
    /// </summary>
    Hybrid = 4
}
