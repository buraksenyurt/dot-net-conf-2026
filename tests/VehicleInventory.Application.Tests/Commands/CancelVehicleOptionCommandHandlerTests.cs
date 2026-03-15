using FluentAssertions;
using Moq;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Commands;

public class CancelVehicleOptionCommandHandlerTests
{
    private readonly Mock<IVehicleOptionRepository> _vehicleOptionRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly CancelVehicleOptionCommandHandler _handler;

    public CancelVehicleOptionCommandHandlerTests()
    {
        _vehicleOptionRepositoryMock = new Mock<IVehicleOptionRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();

        _handler = new CancelVehicleOptionCommandHandler(
            _vehicleOptionRepositoryMock.Object,
            _vehicleRepositoryMock.Object);
    }

    private static VehicleOption CreateActiveOption()
    {
        var vin = VIN.Create("1HGBH41JXMN109186").Value!;
        var spec = new VehicleSpecification("Toyota", "Corolla", 2023, EngineType.Gasoline, 0, "White", TransmissionType.Automatic, 6.5m, 1600);
        var purchasePrice = Money.Create(100000, "TRY").Value!;
        var suggestedPrice = Money.Create(120000, "TRY").Value!;
        var vehicle = Vehicle.Create(vin, spec, purchasePrice, suggestedPrice).Value!;

        var emailResult = Email.Create("test@example.com").Value!;
        var customer = Customer.CreateIndividual("John", "Doe", emailResult, "5551234567").Value!;

        var optionFee = Money.Create(1000, "TRY").Value!;
        return VehicleOption.Create(vehicle, customer, 7, optionFee).Value!;
    }

    [Fact]
    public async Task Handle_OptionNotFound_ReturnsFailure()
    {
        var optionId = Guid.NewGuid();
        var command = new CancelVehicleOptionCommand(optionId);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetByIdAsync(optionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleOption?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain(optionId.ToString());
    }

    [Fact]
    public async Task Handle_ValidActiveOption_ReturnsSuccess()
    {
        var option = CreateActiveOption();
        var command = new CancelVehicleOptionCommand(option.Id);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetByIdAsync(option.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(option);

        _vehicleRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        _vehicleOptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<VehicleOption>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidActiveOption_UpdateAsyncCalledForVehicle()
    {
        var option = CreateActiveOption();
        var command = new CancelVehicleOptionCommand(option.Id);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetByIdAsync(option.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(option);

        _vehicleRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        _vehicleOptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<VehicleOption>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _vehicleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidActiveOption_UpdateAsyncCalledForOption()
    {
        var option = CreateActiveOption();
        var command = new CancelVehicleOptionCommand(option.Id);

        _vehicleOptionRepositoryMock
            .Setup(r => r.GetByIdAsync(option.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(option);

        _vehicleRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        _vehicleOptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<VehicleOption>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _vehicleOptionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<VehicleOption>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
