using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Commands;

public class AddVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _repositoryMock;
    private readonly Mock<ILogger<AddVehicleCommandHandler>> _loggerMock;
    private readonly AddVehicleCommandHandler _handler;

    public AddVehicleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<AddVehicleCommandHandler>>();
        _handler = new AddVehicleCommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    private static AddVehicleCommand CreateValidCommand(string vin = "1HGBH41JXMN109186")
        => new AddVehicleCommand(
            Vin: vin,
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

    private static Vehicle CreateVehicleFromCommand(AddVehicleCommand command)
    {
        var vin = VIN.Create(command.Vin).Value!;
        var spec = new VehicleSpecification(
            command.Brand, command.Model, command.Year,
            EngineType.Gasoline, command.Mileage, command.Color,
            TransmissionType.Automatic, command.FuelConsumption, command.EngineCapacity);
        var purchase = Money.Create(command.PurchaseAmount, command.PurchaseCurrency).Value!;
        var suggested = Money.Create(command.SuggestedAmount, command.SuggestedCurrency).Value!;
        return Vehicle.Create(vin, spec, purchase, suggested).Value!;
    }

    [Fact]
    public async Task Handle_ValidCommand_ExistsReturnsFalse_ReturnsSuccess()
    {
        var command = CreateValidCommand();
        var vehicle = CreateVehicleFromCommand(command);

        _repositoryMock
            .Setup(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_ExistsCalledWithVin()
    {
        var command = CreateValidCommand();
        var vehicle = CreateVehicleFromCommand(command);

        _repositoryMock
            .Setup(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddAsyncCalledOnce()
    {
        var command = CreateValidCommand();
        var vehicle = CreateVehicleFromCommand(command);

        _repositoryMock
            .Setup(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle v, CancellationToken _) => v);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistsReturnsTrue_ReturnsFailure()
    {
        var command = CreateValidCommand();

        _repositoryMock
            .Setup(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain(command.Vin);
    }

    [Fact]
    public async Task Handle_ExistsReturnsTrue_AddAsyncNotCalled()
    {
        var command = CreateValidCommand();

        _repositoryMock
            .Setup(r => r.ExistsAsync(command.Vin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidVinFormat_ReturnsFailure()
    {
        var command = CreateValidCommand(vin: "INVALID-VIN-12345");

        _repositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InvalidVinWithIChar_ReturnsFailure()
    {
        // VIN with 'I' character - but first ExistsAsync returns false
        var command = CreateValidCommand(vin: "1HGBH41JXMN10918I");

        _repositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
