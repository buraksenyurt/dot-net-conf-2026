using FluentAssertions;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.Entities;

public class VehicleOptionTests
{
    #region Helpers

    private static Vehicle CreateInStockVehicle()
    {
        var vin = VIN.Create("1HGBH41JXMN109186").Value!;
        var spec = new VehicleSpecification("Toyota", "Corolla", 2020, EngineType.Gasoline, 0, "White", TransmissionType.Automatic, 6.5m, 1600);
        var purchase = Money.Create(100000, "TRY").Value!;
        var suggested = Money.Create(120000, "TRY").Value!;
        return Vehicle.Create(vin, spec, purchase, suggested).Value!;
    }

    private static Customer CreateCustomer()
    {
        var email = Email.Create("test@example.com").Value!;
        return Customer.CreateIndividual("John", "Doe", email, "5551234567").Value!;
    }

    private static Money CreateOptionFee(decimal amount = 1000)
        => Money.Create(amount, "TRY").Value!;

    #endregion

    [Fact]
    public void Create_WithInStockVehicle_ReturnsSuccess()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        var result = VehicleOption.Create(vehicle, customer, 7, fee);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithSoldVehicle_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        vehicle.ChangeStatus(VehicleStatus.Sold);
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        var result = VehicleOption.Create(vehicle, customer, 7, fee);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("sold");
    }

    [Fact]
    public void Create_WithReservedVehicle_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        vehicle.ChangeStatus(VehicleStatus.Reserved);
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        var result = VehicleOption.Create(vehicle, customer, 7, fee);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("reserved");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ValidityDaysLessThan1_ReturnsFailure(int days)
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        var result = VehicleOption.Create(vehicle, customer, days, fee);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("1 and 30");
    }

    [Fact]
    public void Create_ValidityDaysGreaterThan30_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        var result = VehicleOption.Create(vehicle, customer, 31, fee);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("1 and 30");
    }

    [Fact]
    public void Create_SetsVehicleStatusToReserved()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();

        VehicleOption.Create(vehicle, customer, 7, fee);

        vehicle.Status.Should().Be(VehicleStatus.Reserved);
    }

    [Fact]
    public void Cancel_ActiveOption_ReturnsSuccess()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;

        var result = option.Cancel();

        result.IsSuccess.Should().BeTrue();
        option.Status.Should().Be(VehicleOptionStatus.Cancelled);
    }

    [Fact]
    public void Cancel_SetsVehicleStatusToOnSale()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;

        option.Cancel();

        vehicle.Status.Should().Be(VehicleStatus.OnSale);
    }

    [Fact]
    public void Cancel_AlreadyCancelledOption_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;
        option.Cancel();

        var result = option.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("active");
    }

    [Fact]
    public void IsExpired_NonExpiredOption_ReturnsFalse()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;

        option.IsExpired().Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ExpiredOption_ReturnsTrue()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;

        // The entity uses DateTime.UtcNow directly with no ITimeProvider injection.
        // Reflection on the private setter is the least-fragile way to simulate a past
        // expiry without modifying production code or introducing Thread.Sleep.
        typeof(VehicleOption)
            .GetProperty(nameof(VehicleOption.ExpiresAt))!
            .SetValue(option, DateTime.UtcNow.AddDays(-1));

        option.IsExpired().Should().BeTrue();
    }

    [Fact]
    public void IsExpired_CancelledOptionWithPastDate_ReturnsFalse()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var fee = CreateOptionFee();
        var option = VehicleOption.Create(vehicle, customer, 7, fee).Value!;
        option.Cancel();

        // Set date to past - but since status is Cancelled, IsExpired should return false
        typeof(VehicleOption)
            .GetProperty(nameof(VehicleOption.ExpiresAt))!
            .SetValue(option, DateTime.UtcNow.AddDays(-1));

        option.IsExpired().Should().BeFalse();
    }
}
