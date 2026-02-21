using System.Text.RegularExpressions;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Domain.ValueObjects;

/// <summary>
/// Email address value object.
/// Encapsulates validation and normalization of e-mail addresses.
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    // Parameterless constructor for EF Core
    private Email() : this(string.Empty) { }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method that validates and creates an Email value object.
    /// </summary>
    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure("Email address cannot be empty");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 254)
            return Result<Email>.Failure("Email address cannot exceed 254 characters");

        if (!EmailRegex.IsMatch(normalized))
            return Result<Email>.Failure("Email address is not in a valid format");

        return Result<Email>.Success(new Email(normalized));
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
}
