using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Domain.Entities;

/// <summary>
/// VehicleOption entity — represents a purchase option (reservation) of a vehicle for a customer.
/// US-003: Araç Opsiyonlama.
/// Business rules:
///   - Vehicle must be InStock or OnSale to be optioned.
///   - ValidityDays must be between 1 and 30.
///   - OptionFee amount must be >= 0.
///   - Only one active option is allowed per vehicle at a time (enforced at repository level).
///   - Creating an option changes the vehicle status to Reserved.
///   - Cancelling an option changes the vehicle status back to OnSale.
/// </summary>
public sealed class VehicleOption
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public Guid? ServiceAdvisorId { get; private set; }
    public ServiceAdvisor? ServiceAdvisor { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public Money OptionFee { get; private set; } = null!;
    public string? Notes { get; private set; }
    public VehicleOptionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private VehicleOption() { }

    private VehicleOption(
        Guid id,
        Vehicle vehicle,
        Customer customer,
        DateTime expiresAt,
        Money optionFee,
        Guid? serviceAdvisorId,
        string? notes)
    {
        Id = id;
        VehicleId = vehicle.Id;
        Vehicle = vehicle;
        CustomerId = customer.Id;
        Customer = customer;
        ServiceAdvisorId = serviceAdvisorId;
        ExpiresAt = expiresAt;
        OptionFee = optionFee;
        Notes = notes?.Trim();
        Status = VehicleOptionStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method. Creates an active option and marks the vehicle as Reserved.
    /// </summary>
    public static Result<VehicleOption> Create(
        Vehicle vehicle,
        Customer customer,
        int validityDays,
        Money optionFee,
        Guid? serviceAdvisorId = null,
        string? notes = null)
    {
        if (vehicle.Status == VehicleStatus.Reserved)
            return Result<VehicleOption>.Failure("Vehicle is already reserved by another option");

        if (vehicle.Status == VehicleStatus.Sold)
            return Result<VehicleOption>.Failure("Cannot option a sold vehicle");

        if (validityDays < 1 || validityDays > 30)
            return Result<VehicleOption>.Failure("Validity period must be between 1 and 30 days");

        if (optionFee.Amount < 0)
            return Result<VehicleOption>.Failure("Option fee cannot be negative");

        // Mark vehicle as reserved
        var statusResult = vehicle.ChangeStatus(VehicleStatus.Reserved);
        if (statusResult.IsFailure)
            return Result<VehicleOption>.Failure(statusResult.Error);

        var option = new VehicleOption(
            Guid.NewGuid(),
            vehicle,
            customer,
            DateTime.UtcNow.AddDays(validityDays),
            optionFee,
            serviceAdvisorId,
            notes);

        return Result<VehicleOption>.Success(option);
    }

    /// <summary>
    /// Cancels this option and frees the vehicle back to OnSale.
    /// </summary>
    public Result<VehicleOption> Cancel()
    {
        if (Status != VehicleOptionStatus.Active)
            return Result<VehicleOption>.Failure("Only active options can be cancelled");

        Status = VehicleOptionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        // Return vehicle to OnSale
        Vehicle.ChangeStatus(VehicleStatus.OnSale);

        return Result<VehicleOption>.Success(this);
    }

    /// <summary>
    /// Returns true if the option has passed its expiry date (but Status is still Active in DB).
    /// </summary>
    public bool IsExpired() =>
        Status == VehicleOptionStatus.Active && DateTime.UtcNow > ExpiresAt;
}
