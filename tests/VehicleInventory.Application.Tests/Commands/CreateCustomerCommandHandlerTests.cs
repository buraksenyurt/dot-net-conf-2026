using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Commands;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
        _handler = new CreateCustomerCommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    private static CreateCustomerCommand CreateIndividualCommand(string email = "john.doe@example.com")
        => new CreateCustomerCommand(
            FirstName: "John",
            LastName: "Doe",
            Email: email,
            Phone: "5551234567",
            CustomerType: "Individual"
        );

    private static CreateCustomerCommand CreateCorporateCommand(string email = "corp@example.com")
        => new CreateCustomerCommand(
            FirstName: "Jane",
            LastName: "Smith",
            Email: email,
            Phone: "5559876543",
            CustomerType: "Corporate",
            CompanyName: "Acme Corp",
            TaxNumber: "1234567890"
        );

    [Fact]
    public async Task Handle_ValidIndividualCustomer_ReturnsSuccess()
    {
        var command = CreateIndividualCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCorporateCustomer_ReturnsSuccess()
    {
        var command = CreateCorporateCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsFailure()
    {
        var command = CreateIndividualCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain(command.Email);
    }

    [Fact]
    public async Task Handle_ValidCommand_ExistsByEmailCalled()
    {
        var command = CreateIndividualCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddAsyncCalledOnSuccess()
    {
        var command = CreateIndividualCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_AddAsyncNotCalled()
    {
        var command = CreateIndividualCommand();

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
