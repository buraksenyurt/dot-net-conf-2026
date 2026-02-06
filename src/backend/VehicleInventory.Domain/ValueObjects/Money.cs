using VehicleInventory.Domain.Common;

namespace VehicleInventory.Domain.ValueObjects;

/// <summary>
/// Money value object representing an amount with currency (ISO 4217).
/// Supports arithmetic operations with currency validation.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Result<Money>.Failure("Amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure("Currency cannot be empty");

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
            return Result<Money>.Failure("Currency code must be 3 characters (ISO 4217)");

        // Common currency codes validation
        var validCurrencies = new[] { "TRY", "USD", "EUR", "GBP", "JPY", "CHF" };
        if (!validCurrencies.Contains(normalizedCurrency))
            return Result<Money>.Failure($"Unsupported currency: {normalizedCurrency}");

        return Result<Money>.Success(new Money(amount, normalizedCurrency));
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

        if (Amount - other.Amount < 0)
            throw new InvalidOperationException("Result would be negative");

        return new Money(Amount - other.Amount, Currency);
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static bool operator ==(Money? left, Money? right) => Equals(left, right);
    public static bool operator !=(Money? left, Money? right) => !Equals(left, right);
}
