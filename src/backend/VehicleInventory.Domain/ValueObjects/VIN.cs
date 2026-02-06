using VehicleInventory.Domain.Common;

namespace VehicleInventory.Domain.ValueObjects;

/// <summary>
/// Vehicle Identification Number (VIN) value object.
/// Represents a standardized 17-character identifier for vehicles conforming to ISO 3779.
/// </summary>
public sealed class VIN : IEquatable<VIN>
{
    public string Value { get; }

    private VIN(string value)
    {
        Value = value;
    }

    public static Result<VIN> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<VIN>.Failure("VIN cannot be empty");

        var cleaned = value.Trim().ToUpperInvariant();

        if (cleaned.Length != 17)
            return Result<VIN>.Failure("VIN must be exactly 17 characters");

        // Check for invalid characters (I, O, Q are not allowed in VIN per ISO 3779)
        if (cleaned.Any(c => c == 'I' || c == 'O' || c == 'Q'))
            return Result<VIN>.Failure("VIN cannot contain I, O, or Q characters");

        // Check if all characters are alphanumeric
        if (!cleaned.All(c => char.IsLetterOrDigit(c)))
            return Result<VIN>.Failure("VIN must contain only letters and numbers");

        return Result<VIN>.Success(new VIN(cleaned));
    }

    public bool Equals(VIN? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is VIN other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(VIN? left, VIN? right) => Equals(left, right);
    public static bool operator !=(VIN? left, VIN? right) => !Equals(left, right);
}
