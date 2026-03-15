using FluentAssertions;
using Moq;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Commands;

public class CreateVehicleOptionCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IVehicleOptionRepository> _vehicleOptionRepositoryMock;
    private readonly CreateVehicleOptionCommandHandler _handler;

    public CreateVehicleOptionCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _vehicleOptionRepositoryMock = new Mock<IVehicleOptionRepository>();

        _handler = new CreateVehicleOptionCommandHandler(
            _vehicleRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _vehicleOptionRepositoryMock.Object);
    }

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

    private static CreateVehicleOptionCommand CreateCommand(Guid vehicleId, Guid customerId)
        => new CreateVehicleOptionCommand(
            VehicleId: vehicleId,
            CustomerId: customerId,
            ValidityDays: 7,
            OptionFeeAmount: 1000,
            OptionFeeCurrency: "TRY"
        );

    [Fact]
    public async Task Handle_VehicleNotFound_ReturnsFailure()
    {
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = CreateCommand(vehicleId, customerId);

        _vehicleRepositoryMock
            .Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain(vehicleId.ToString());
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        var customerId = Guid.NewGuid();
        var command = CreateCommand(vehicle.Id, customerId);

        _vehicleRepositoryMock
            .Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain(customerId.ToString());
    }

    [Fact]
    public async Task Handle_ActiveOptionAlreadyExists_ReturnsFailure()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var command = CreateCommand(vehicle.Id, customer.Id);

        // Create an existing active option (need to use a different vehicle for it)
        var otherVehicle = CreateInStockVehicle();
        var fee = Money.Create(1000, "TRY").Value!;
        var existingOption = VehicleOption.Create(otherVehicle, customer, 7, fee).Value!;

        _vehicleRepositoryMock
            .Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetActiveByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOption);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("active option");
    }

    [Fact]
    public async Task Handle_ValidVehicleAndCustomerNoActiveOption_ReturnsSuccess()
    {
        var vehicle = CreateInStockVehicle();
        var customer = CreateCustomer();
        var command = CreateCommand(vehicle.Id, customer.Id);

        _vehicleRepositoryMock
            .Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _vehicleRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetActiveByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleOption?)null);

        _vehicleOptionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<VehicleOption>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleOption o, CancellationToken _) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
