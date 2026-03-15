using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleInventory.Application.Queries;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Queries;

public class GetCustomersQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<ILogger<GetCustomersQueryHandler>> _loggerMock;
    private readonly GetCustomersQueryHandler _handler;

    public GetCustomersQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<GetCustomersQueryHandler>>();
        _handler = new GetCustomersQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    private static Customer CreateIndividualCustomer(string firstName = "John", string lastName = "Doe", string email = "john.doe@example.com", string phone = "5551234567")
    {
        var emailObj = Email.Create(email).Value!;
        return Customer.CreateIndividual(firstName, lastName, emailObj, phone).Value!;
    }

    private static Customer CreateCorporateCustomer(string email = "corp@example.com")
    {
        var emailObj = Email.Create(email).Value!;
        return Customer.CreateCorporate("Jane", "Smith", emailObj, "5559876543", "Acme Corp", "1234567890").Value!;
    }

    [Fact]
    public async Task Handle_RepositoryReturnsCustomers_ReturnsPagedResultWithCorrectCount()
    {
        var customer1 = CreateIndividualCustomer("John", "Doe", "john@example.com");
        var customer2 = CreateIndividualCustomer("Jane", "Smith", "jane@example.com");
        var customers = new List<Customer> { customer1, customer2 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((customers as IEnumerable<Customer>, 2));

        var query = new GetCustomersQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Count().Should().Be(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsPagedResultWithZeroItems()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enumerable.Empty<Customer>(), 0));

        var query = new GetCustomersQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_IndividualCustomer_MapsEmailAndDisplayNameCorrectly()
    {
        var customer = CreateIndividualCustomer("John", "Doe", "john.doe@example.com");

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Customer> { customer } as IEnumerable<Customer>, 1));

        var query = new GetCustomersQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.Email.Should().Be("john.doe@example.com");
        dto.DisplayName.Should().Be("John Doe");
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.CustomerType.Should().Be("Individual");
    }

    [Fact]
    public async Task Handle_CorporateCustomer_MapsDisplayNameCorrectly()
    {
        var customer = CreateCorporateCustomer("corp@example.com");

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Customer> { customer } as IEnumerable<Customer>, 1));

        var query = new GetCustomersQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.DisplayName.Should().Be("Acme Corp (Jane Smith)");
        dto.CompanyName.Should().Be("Acme Corp");
        dto.CustomerType.Should().Be("Corporate");
    }

    [Fact]
    public async Task Handle_TotalPagesCalculatedCorrectly()
    {
        var customers = Enumerable.Range(0, 3).Select(i =>
            CreateIndividualCustomer("John", "Doe", $"user{i}@example.com")).ToList();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((customers as IEnumerable<Customer>, 15));

        var query = new GetCustomersQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalPages.Should().Be(2); // ceiling(15/10) = 2
    }
}
