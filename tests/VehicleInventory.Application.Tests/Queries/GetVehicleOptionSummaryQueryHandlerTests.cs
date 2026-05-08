using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Application.Queries;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Application.Tests.Queries;

public class GetVehicleOptionSummaryQueryHandlerTests
{
    private readonly Mock<IVehicleOptionRepository> _repositoryMock = new();
    private readonly Mock<ILogger<GetVehicleOptionSummaryQueryHandler>> _loggerMock = new();
    private readonly GetVehicleOptionSummaryQueryHandler _handler;

    public GetVehicleOptionSummaryQueryHandlerTests()
    {
        _handler = new GetVehicleOptionSummaryQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Vehicle CreateVehicle(string vin = "1HGBH41JXMN109186")
    {
        var vinObj = VIN.Create(vin).Value!;
        var spec = new VehicleSpecification("Honda", "Civic", 2026, EngineType.Gasoline, 0, "White", TransmissionType.Automatic, 5.5m, 1500);
        var purchase = Money.Create(400_000, "TRY").Value!;
        var suggested = Money.Create(450_000, "TRY").Value!;
        return Vehicle.Create(vinObj, spec, purchase, suggested).Value!;
    }

    private static Customer CreateCustomer(string firstName = "Ahmet", string lastName = "Yılmaz", string email = "ahmet@example.com")
    {
        var emailObj = Email.Create(email).Value!;
        return Customer.CreateIndividual(firstName, lastName, emailObj, "5551234567").Value!;
    }

    private static VehicleOption CreateActiveOption(Vehicle vehicle, Customer customer, int validityDays = 7)
        => VehicleOption.Create(vehicle, customer, validityDays, Money.Create(5000, "TRY").Value!).Value!;

    private void SetupRepository(IEnumerable<VehicleOption> items, int totalCount)
    {
        _repositoryMock
            .Setup(r => r.GetSummaryAsync(
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<VehicleOptionStatus?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));
    }

    // ── Tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidQuery_ReturnsMappedPagedResult()
    {
        var vehicle = CreateVehicle();
        var customer = CreateCustomer();
        var option = CreateActiveOption(vehicle, customer);
        SetupRepository(new[] { option }, 1);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ActiveOptionNotExpired_StatusIsActive()
    {
        var vehicle = CreateVehicle();
        var customer = CreateCustomer();
        var option = CreateActiveOption(vehicle, customer, validityDays: 30);
        SetupRepository(new[] { option }, 1);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.IsExpired.Should().BeFalse();
        dto.Status.Should().Be((int)VehicleOptionStatus.Active);
    }

    [Fact]
    public async Task Handle_ActiveOptionAlreadyExpired_StatusMappedToExpired()
    {
        // Build a vehicle option and then use reflection to back-date ExpiresAt
        var vehicle = CreateVehicle();
        var customer = CreateCustomer();
        var option = CreateActiveOption(vehicle, customer, validityDays: 1);

        // Force ExpiresAt to be in the past via reflection (BR-2: DB status stays Active)
        var expiresAtProp = typeof(VehicleOption).GetProperty("ExpiresAt")!;
        expiresAtProp.SetValue(option, DateTime.UtcNow.AddDays(-1));

        SetupRepository(new[] { option }, 1);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.IsExpired.Should().BeTrue();
        dto.Status.Should().Be((int)VehicleOptionStatus.Expired);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResult()
    {
        SetupRepository(Enumerable.Empty<VehicleOption>(), 0);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_TotalPagesCalculatedCorrectly()
    {
        var vehicle = CreateVehicle();
        var customer = CreateCustomer();
        var options = Enumerable.Range(0, 5)
            .Select(_ => CreateActiveOption(CreateVehicle(), CreateCustomer()))
            .ToList();

        SetupRepository(options, 45);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null, Page: 1, PageSize: 20);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalPages.Should().Be(3); // ceil(45/20) = 3
    }

    [Fact]
    public async Task Handle_DtoFields_MappedCorrectly()
    {
        var vehicle = CreateVehicle();
        var customer = CreateCustomer("Ahmet", "Yılmaz");
        var option = CreateActiveOption(vehicle, customer);
        SetupRepository(new[] { option }, 1);

        var query = new GetVehicleOptionSummaryQuery(null, null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        var dto = result.Items.First();
        dto.VehicleDisplayName.Should().Be("Honda Civic 2026");
        dto.VehicleVIN.Should().Be("1HGBH41JXMN109186");
        dto.CustomerDisplayName.Should().Be("Ahmet Yılmaz");
        dto.OptionFeeAmount.Should().Be(5000);
        dto.OptionFeeCurrency.Should().Be("TRY");
        dto.ServiceAdvisorDisplayName.Should().BeNull();
    }

    [Fact]
    public async Task Handle_RepositoryCalledWithCorrectParameters()
    {
        SetupRepository(Enumerable.Empty<VehicleOption>(), 0);

        var query = new GetVehicleOptionSummaryQuery(
            CustomerSearch: "ahmet",
            VehicleSearch: "honda",
            Status: VehicleOptionStatus.Active,
            CreatedFrom: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedTo: new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            Page: 2,
            PageSize: 10,
            SortBy: "createdAt",
            SortDirection: "desc");

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetSummaryAsync(
            "ahmet",
            "honda",
            VehicleOptionStatus.Active,
            query.CreatedFrom,
            query.CreatedTo,
            2,
            10,
            "createdAt",
            "desc",
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
