using FluentAssertions;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.Entities;

public class CustomerTests
{
    private static Email CreateValidEmail(string value = "test@example.com")
        => Email.Create(value).Value!;

    [Fact]
    public void CreateIndividual_ValidData_ReturnsSuccess()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateIndividual("John", "Doe", email, "5551234567");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FirstName.Should().Be("John");
        result.Value!.LastName.Should().Be("Doe");
    }

    [Fact]
    public void CreateIndividual_EmptyFirstName_ReturnsFailure()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateIndividual("", "Doe", email, "5551234567");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("First name");
    }

    [Fact]
    public void CreateIndividual_EmptyLastName_ReturnsFailure()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateIndividual("John", "", email, "5551234567");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Last name");
    }

    [Fact]
    public void CreateIndividual_EmptyPhone_ReturnsFailure()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateIndividual("John", "Doe", email, "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Phone");
    }

    [Fact]
    public void CreateCorporate_ValidData_ReturnsSuccess()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateCorporate("John", "Doe", email, "5551234567", "Acme Corp", "1234567890");

        result.IsSuccess.Should().BeTrue();
        result.Value!.CompanyName.Should().Be("Acme Corp");
        result.Value!.TaxNumber.Should().Be("1234567890");
    }

    [Fact]
    public void CreateCorporate_EmptyCompanyName_ReturnsFailure()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateCorporate("John", "Doe", email, "5551234567", "", "1234567890");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Company name");
    }

    [Fact]
    public void CreateCorporate_EmptyTaxNumber_ReturnsFailure()
    {
        var email = CreateValidEmail();

        var result = Customer.CreateCorporate("John", "Doe", email, "5551234567", "Acme Corp", "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Tax number");
    }

    [Fact]
    public void GetDisplayName_CorporateCustomer_ReturnsCompanyNameWithFullName()
    {
        var email = CreateValidEmail();
        var customer = Customer.CreateCorporate("John", "Doe", email, "5551234567", "Acme Corp", "1234567890").Value!;

        var displayName = customer.GetDisplayName();

        displayName.Should().Be("Acme Corp (John Doe)");
    }

    [Fact]
    public void GetDisplayName_IndividualCustomer_ReturnsFirstLastName()
    {
        var email = CreateValidEmail();
        var customer = Customer.CreateIndividual("John", "Doe", email, "5551234567").Value!;

        var displayName = customer.GetDisplayName();

        displayName.Should().Be("John Doe");
    }

    [Fact]
    public void UpdateContact_ValidData_UpdatesValues()
    {
        var email = CreateValidEmail();
        var customer = Customer.CreateIndividual("John", "Doe", email, "5551234567").Value!;
        var newEmail = Email.Create("newemail@example.com").Value!;

        var result = customer.UpdateContact("9998887766", newEmail);

        result.IsSuccess.Should().BeTrue();
        customer.Phone.Should().Be("9998887766");
        customer.Email.Value.Should().Be("newemail@example.com");
    }

    [Fact]
    public void UpdateContact_EmptyPhone_ReturnsFailure()
    {
        var email = CreateValidEmail();
        var customer = Customer.CreateIndividual("John", "Doe", email, "5551234567").Value!;
        var newEmail = Email.Create("newemail@example.com").Value!;

        var result = customer.UpdateContact("", newEmail);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Phone");
    }
}
