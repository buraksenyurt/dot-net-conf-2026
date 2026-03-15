using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleInventory.Application.Queries;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Queries;

public class GetVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _repositoryMock;
    private readonly Mock<ILogger<GetVehiclesQueryHandler>> _loggerMock;
    private readonly GetVehiclesQueryHandler _handler;

    public GetVehiclesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<GetVehiclesQueryHandler>>();
        _handler = new GetVehiclesQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    private static Vehicle CreateVehicle(string vinValue = "1HGBH41JXMN109186")
    {
        var vin = VIN.Create(vinValue).Value!;
        var spec = new VehicleSpecification("Toyota", "Corolla", 2020, EngineType.Gasoline, 0, "White", TransmissionType.Automatic, 6.5m, 1600);
        var purchase = Money.Create(100000, "TRY").Value!;
        var suggested = Money.Create(120000, "TRY").Value!;
        return Vehicle.Create(vin, spec, purchase, suggested).Value!;
    }

    [Fact]
    public async Task Handle_RepositoryReturnsVehicles_ReturnsPagedResultWithCorrectCount()
    {
        var vehicle1 = CreateVehicle("1HGBH41JXMN109186");
        var vehicle2 = CreateVehicle("2T1BURHE0JC043821");
        var vehicles = new List<Vehicle> { vehicle1, vehicle2 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((vehicles as IEnumerable<Vehicle>, 2));

        var query = new GetVehiclesQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Count().Should().Be(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsPagedResultWithZeroItems()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enumerable.Empty<Vehicle>(), 0));

        var query = new GetVehiclesQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsVehicle_MapsToVehicleDto()
    {
        var vehicle = CreateVehicle("1HGBH41JXMN109186");

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Vehicle> { vehicle } as IEnumerable<Vehicle>, 1));

        var query = new GetVehiclesQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.VIN.Should().Be("1HGBH41JXMN109186");
        dto.Brand.Should().Be("Toyota");
        dto.Model.Should().Be("Corolla");
        dto.Status.Should().Be("InStock");
    }

    [Fact]
    public async Task Handle_TotalPageCalculatedCorrectly()
    {
        var vehicles = Enumerable.Range(0, 5).Select(i =>
            CreateVehicle($"1HGBH41JXMN10918{i}")).ToList();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((vehicles as IEnumerable<Vehicle>, 25));

        var query = new GetVehiclesQuery(Page: 1, PageSize: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalPages.Should().Be(3); // ceiling(25/10) = 3
    }
}
