namespace VehicleInventory.Domain.Enums;

/// <summary>
/// Represents the current status of a vehicle in the inventory.
/// </summary>
public enum VehicleStatus
{
    /// <summary>
    /// Vehicle is in stock, not yet available for sale
    /// </summary>
    InStock = 1,

    /// <summary>
    /// Vehicle is available for sale
    /// </summary>
    OnSale = 2,

    /// <summary>
    /// Vehicle has been sold
    /// </summary>
    Sold = 3,

    /// <summary>
    /// Vehicle is reserved by a customer
    /// </summary>
    Reserved = 4
}
