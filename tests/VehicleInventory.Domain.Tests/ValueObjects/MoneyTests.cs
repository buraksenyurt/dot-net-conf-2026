using FluentAssertions;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(100, "TRY")]
    [InlineData(50.5, "USD")]
    [InlineData(200, "EUR")]
    [InlineData(150, "GBP")]
    [InlineData(10000, "JPY")]
    [InlineData(75, "CHF")]
    public void Create_ValidAmountAndCurrency_ReturnsSuccess(decimal amount, string currency)
    {
        var result = Money.Create(amount, currency);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(amount);
        result.Value!.Currency.Should().Be(currency);
    }

    [Fact]
    public void Create_NegativeAmount_ReturnsFailure()
    {
        var result = Money.Create(-1, "TRY");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("negative");
    }

    [Fact]
    public void Create_ZeroAmount_ReturnsSuccess()
    {
        var result = Money.Create(0, "TRY");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(0);
    }

    [Fact]
    public void Create_EmptyCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, string.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("empty");
    }

    [Fact]
    public void Create_WhitespaceCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, "   ");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_UnsupportedCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, "XYZ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Unsupported currency");
    }

    [Fact]
    public void Create_Non3CharCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, "TR");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("3 characters");
    }

    [Fact]
    public void Create_LowercaseCurrency_NormalizesToUppercase()
    {
        var result = Money.Create(100, "try");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        var money1 = Money.Create(100, "TRY").Value!;
        var money2 = Money.Create(200, "TRY").Value!;

        var result = money1.Add(money2);

        result.Amount.Should().Be(300);
        result.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Add_DifferentCurrencies_ThrowsInvalidOperationException()
    {
        var money1 = Money.Create(100, "TRY").Value!;
        var money2 = Money.Create(200, "USD").Value!;

        Action act = () => money1.Add(money2);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Subtract_SameCurrency_ReturnsDifference()
    {
        var money1 = Money.Create(300, "TRY").Value!;
        var money2 = Money.Create(100, "TRY").Value!;

        var result = money1.Subtract(money2);

        result.Amount.Should().Be(200);
        result.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Subtract_DifferentCurrencies_ThrowsInvalidOperationException()
    {
        var money1 = Money.Create(300, "TRY").Value!;
        var money2 = Money.Create(100, "USD").Value!;

        Action act = () => money1.Subtract(money2);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Subtract_WhenResultIsNegative_ThrowsInvalidOperationException()
    {
        var money1 = Money.Create(50, "TRY").Value!;
        var money2 = Money.Create(100, "TRY").Value!;

        Action act = () => money1.Subtract(money2);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equals_TwoMoneyWithSameAmountAndCurrency_AreEqual()
    {
        var money1 = Money.Create(100, "TRY").Value!;
        var money2 = Money.Create(100, "TRY").Value!;

        money1.Should().Be(money2);
        money1.Equals(money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoMoneyWithDifferentAmounts_AreNotEqual()
    {
        var money1 = Money.Create(100, "TRY").Value!;
        var money2 = Money.Create(200, "TRY").Value!;

        money1.Should().NotBe(money2);
    }
}
