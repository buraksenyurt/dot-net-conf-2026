using FluentAssertions;
using FluentValidation.TestHelper;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Validators;
using Xunit;

namespace VehicleInventory.Application.Tests.Validators;

public class AddVehicleCommandValidatorTests
{
    private readonly AddVehicleCommandValidator _validator;

    public AddVehicleCommandValidatorTests()
    {
        _validator = new AddVehicleCommandValidator();
    }

    private static AddVehicleCommand CreateValidCommand()
        => new AddVehicleCommand(
            Vin: "1HGBH41JXMN109186",
            Brand: "Toyota",
            Model: "Corolla",
            Year: 2020,
            EngineType: "Gasoline",
            Mileage: 0,
            Color: "White",
            PurchaseAmount: 100000,
            PurchaseCurrency: "TRY",
            SuggestedAmount: 120000,
            SuggestedCurrency: "TRY",
            TransmissionType: "Automatic",
            FuelConsumption: 6.5m,
            EngineCapacity: 1600
        );

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = CreateValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyVin_FailsValidation()
    {
        var command = CreateValidCommand() with { Vin = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Validate_VinLessThan17Chars_FailsValidation()
    {
        var command = CreateValidCommand() with { Vin = "1HGBH41JXMN1091" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Validate_VinMoreThan17Chars_FailsValidation()
    {
        var command = CreateValidCommand() with { Vin = "1HGBH41JXMN109186X" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Validate_VinContainsI_FailsValidation()
    {
        var command = CreateValidCommand() with { Vin = "1HGBH41JXMN10918I" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Validate_VinContainsO_FailsValidation()
    {
        var command = CreateValidCommand() with { Vin = "1HGBH41JXMN10918O" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Validate_EmptyBrand_FailsValidation()
    {
        var command = CreateValidCommand() with { Brand = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Brand);
    }

    [Fact]
    public void Validate_EmptyModel_FailsValidation()
    {
        var command = CreateValidCommand() with { Model = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void Validate_YearEquals1900_FailsValidation()
    {
        // Year must be > 1900 (GreaterThan(1900)), so 1900 fails
        var command = CreateValidCommand() with { Year = 1900 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Validate_YearGreaterThanCurrentYear_FailsValidation()
    {
        var command = CreateValidCommand() with { Year = DateTime.UtcNow.Year + 1 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Validate_NegativeMileage_FailsValidation()
    {
        var command = CreateValidCommand() with { Mileage = -1 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Mileage);
    }

    [Fact]
    public void Validate_PurchaseAmountZero_FailsValidation()
    {
        // PurchaseAmount must be > 0
        var command = CreateValidCommand() with { PurchaseAmount = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PurchaseAmount);
    }

    [Fact]
    public void Validate_SuggestedAmountLessThanPurchaseAmount_FailsValidation()
    {
        var command = CreateValidCommand() with { PurchaseAmount = 100000, SuggestedAmount = 80000 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SuggestedAmount);
    }

    [Fact]
    public void Validate_SuggestedAmountEqualToPurchaseAmount_PassesValidation()
    {
        var command = CreateValidCommand() with { PurchaseAmount = 100000, SuggestedAmount = 100000 };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.SuggestedAmount);
    }

    [Fact]
    public void Validate_ZeroMileage_PassesValidation()
    {
        var command = CreateValidCommand() with { Mileage = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Mileage);
    }

    [Fact]
    public void Validate_CurrentYear_PassesValidation()
    {
        var command = CreateValidCommand() with { Year = DateTime.UtcNow.Year };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Year);
    }
}
