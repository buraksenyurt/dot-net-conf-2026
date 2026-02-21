namespace VehicleInventory.Domain.Enums;

/// <summary>
/// Represents the current status of a vehicle option (reservation for purchase).
/// </summary>
public enum VehicleOptionStatus
{
    /// <summary>
    /// Option is active and vehicle is reserved for the customer.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Option validity period has expired without conversion or cancellation.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// Option was explicitly cancelled by a sales advisor.
    /// </summary>
    Cancelled = 3
}
