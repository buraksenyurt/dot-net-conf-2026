using FluentValidation;
using VehicleInventory.Application.Commands;

namespace VehicleInventory.Application.Validators;

public class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.Vin)
            .NotEmpty().WithMessage("VIN is required")
            .Length(17).WithMessage("VIN must be exactly 17 characters")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$").WithMessage("VIN contains invalid characters");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PurchaseAmount)
            .GreaterThan(0);

        RuleFor(x => x.SuggestedAmount)
            .GreaterThanOrEqualTo(x => x.PurchaseAmount);
    }
}
