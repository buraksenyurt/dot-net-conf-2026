using MediatR;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Application.Commands;

public class CreateVehicleOptionCommandHandler : IRequestHandler<CreateVehicleOptionCommand, Result<Guid>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleOptionRepository _vehicleOptionRepository;

    public CreateVehicleOptionCommandHandler(
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IVehicleOptionRepository vehicleOptionRepository)
    {
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _vehicleOptionRepository = vehicleOptionRepository;
    }

    public async Task<Result<Guid>> Handle(CreateVehicleOptionCommand request, CancellationToken cancellationToken)
    {
        // Load vehicle
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle is null)
            return Result<Guid>.Failure($"Vehicle with id '{request.VehicleId}' not found");

        // Load customer
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result<Guid>.Failure($"Customer with id '{request.CustomerId}' not found");

        // Ensure no other active option on this vehicle
        var existing = await _vehicleOptionRepository.GetActiveByVehicleIdAsync(vehicle.Id, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure("Vehicle already has an active option");

        // Create money value object
        var moneyResult = Money.Create(request.OptionFeeAmount, request.OptionFeeCurrency);
        if (moneyResult.IsFailure)
            return Result<Guid>.Failure(moneyResult.Error);

        // Create option (also marks vehicle as Reserved)
        var optionResult = VehicleOption.Create(
            vehicle,
            customer,
            request.ValidityDays,
            moneyResult.Value!,
            request.ServiceAdvisorId,
            request.Notes);

        if (optionResult.IsFailure)
            return Result<Guid>.Failure(optionResult.Error);

        var option = optionResult.Value!;

        // Persist vehicle status change first
        await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        // Persist option
        await _vehicleOptionRepository.AddAsync(option, cancellationToken);

        return Result<Guid>.Success(option.Id);
    }
}
