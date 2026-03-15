using FluentAssertions;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.Entities;

public class VehicleTests
{
    #region Helpers

    private static VIN CreateValidVin(string value = "1HGBH41JXMN109186")
        => VIN.Create(value).Value!;

    private static Money CreateMoney(decimal amount, string currency = "TRY")
        => Money.Create(amount, currency).Value!;

    private static VehicleSpecification CreateValidSpec(
        string brand = "Toyota",
        string model = "Corolla",
        int year = 2020,
        EngineType engineType = EngineType.Gasoline,
        int mileage = 0,
        string color = "White",
        TransmissionType transmissionType = TransmissionType.Automatic,
        decimal fuelConsumption = 6.5m,
        int engineCapacity = 1600)
        => new VehicleSpecification(brand, model, year, engineType, mileage, color, transmissionType, fuelConsumption, engineCapacity);

    #endregion

    [Fact]
    public void Create_ValidVehicle_ReturnsSuccess()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec();
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Brand.Should().Be("Toyota");
        result.Value!.Model.Should().Be("Corolla");
        result.Value!.Status.Should().Be(VehicleStatus.InStock);
    }

    [Fact]
    public void Create_SuggestedPriceLessThanPurchasePrice_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec();
        var purchase = CreateMoney(120000);
        var suggested = CreateMoney(100000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Suggested price");
    }

    [Fact]
    public void Create_DifferentCurrencies_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec();
        var purchase = CreateMoney(100000, "TRY");
        var suggested = CreateMoney(120000, "USD");

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("currency");
    }

    [Fact]
    public void Create_NonElectricWithEngineCapacityZero_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(engineType: EngineType.Gasoline, engineCapacity: 0);
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Engine capacity");
    }

    [Fact]
    public void Create_ElectricWithEngineCapacityZero_ReturnsSuccess()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(engineType: EngineType.Electric, engineCapacity: 0);
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_YearBefore1900_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(year: 1899);
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("1900");
    }

    [Fact]
    public void Create_YearGreaterThanCurrentYear_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(year: DateTime.UtcNow.Year + 1);
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_EmptyBrand_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(brand: "");
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Brand");
    }

    [Fact]
    public void Create_EmptyModel_ReturnsFailure()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec(model: "");
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var result = Vehicle.Create(vin, spec, purchase, suggested);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Model");
    }

    [Fact]
    public void Create_StatusStartsAsInStock()
    {
        var vin = CreateValidVin();
        var spec = CreateValidSpec();
        var purchase = CreateMoney(100000);
        var suggested = CreateMoney(120000);

        var vehicle = Vehicle.Create(vin, spec, purchase, suggested).Value!;

        vehicle.Status.Should().Be(VehicleStatus.InStock);
    }

    [Fact]
    public void ChangeStatus_ValidTransition_ReturnsSuccess()
    {
        var vehicle = Vehicle.Create(CreateValidVin(), CreateValidSpec(), CreateMoney(100000), CreateMoney(120000)).Value!;

        var result = vehicle.ChangeStatus(VehicleStatus.OnSale);

        result.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.OnSale);
    }

    [Fact]
    public void ChangeStatus_FromSoldToInStock_ReturnsFailure()
    {
        var vehicle = Vehicle.Create(CreateValidVin(), CreateValidSpec(), CreateMoney(100000), CreateMoney(120000)).Value!;
        vehicle.ChangeStatus(VehicleStatus.Sold);

        var result = vehicle.ChangeStatus(VehicleStatus.InStock);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("sold");
    }

    [Fact]
    public void ChangeStatus_FromSoldToSold_ReturnsSuccess()
    {
        var vehicle = Vehicle.Create(CreateValidVin(), CreateValidSpec(), CreateMoney(100000), CreateMoney(120000)).Value!;
        vehicle.ChangeStatus(VehicleStatus.Sold);

        var result = vehicle.ChangeStatus(VehicleStatus.Sold);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UpdateMileage_LargerMileage_ReturnsSuccess()
    {
        var spec = CreateValidSpec(mileage: 10000);
        var vehicle = Vehicle.Create(CreateValidVin(), spec, CreateMoney(100000), CreateMoney(120000)).Value!;

        var result = vehicle.UpdateMileage(20000);

        result.IsSuccess.Should().BeTrue();
        vehicle.Mileage.Should().Be(20000);
    }

    [Fact]
    public void UpdateMileage_SmallerMileage_ReturnsFailure()
    {
        var spec = CreateValidSpec(mileage: 10000);
        var vehicle = Vehicle.Create(CreateValidVin(), spec, CreateMoney(100000), CreateMoney(120000)).Value!;

        var result = vehicle.UpdateMileage(5000);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("mileage");
    }
}
