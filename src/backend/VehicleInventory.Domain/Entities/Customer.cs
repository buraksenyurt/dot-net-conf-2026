using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Domain.Entities;

/// <summary>
/// Customer aggregate root representing a person or company that can purchase vehicles.
/// Follows Domain-Driven Design principles.
/// </summary>
public sealed class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string Phone { get; private set; }
    public CustomerType CustomerType { get; private set; }

    // Only set for Corporate customers
    public string? CompanyName { get; private set; }
    public string? TaxNumber { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private Customer()
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        Phone = null!;
    }

    private Customer(
        Guid id,
        string firstName,
        string lastName,
        Email email,
        string phone,
        CustomerType customerType,
        string? companyName,
        string? taxNumber)
    {
        Id = id;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email;
        Phone = phone.Trim();
        CustomerType = customerType;
        CompanyName = companyName?.Trim();
        TaxNumber = taxNumber?.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create an Individual customer.
    /// </summary>
    public static Result<Customer> CreateIndividual(
        string firstName,
        string lastName,
        Email email,
        string phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<Customer>.Failure("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<Customer>.Failure("Last name is required");

        if (string.IsNullOrWhiteSpace(phone))
            return Result<Customer>.Failure("Phone number is required");

        var customer = new Customer(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phone,
            CustomerType.Individual,
            companyName: null,
            taxNumber: null);

        return Result<Customer>.Success(customer);
    }

    /// <summary>
    /// Factory method to create a Corporate customer.
    /// </summary>
    public static Result<Customer> CreateCorporate(
        string firstName,
        string lastName,
        Email email,
        string phone,
        string companyName,
        string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<Customer>.Failure("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<Customer>.Failure("Last name is required");

        if (string.IsNullOrWhiteSpace(phone))
            return Result<Customer>.Failure("Phone number is required");

        if (string.IsNullOrWhiteSpace(companyName))
            return Result<Customer>.Failure("Company name is required for corporate customers");

        if (string.IsNullOrWhiteSpace(taxNumber))
            return Result<Customer>.Failure("Tax number is required for corporate customers");

        var customer = new Customer(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phone,
            CustomerType.Corporate,
            companyName,
            taxNumber);

        return Result<Customer>.Success(customer);
    }

    /// <summary>
    /// Updates the contact information of the customer.
    /// </summary>
    public Result<Customer> UpdateContact(string phone, Email email)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result<Customer>.Failure("Phone number is required");

        Phone = phone.Trim();
        Email = email;
        UpdatedAt = DateTime.UtcNow;
        return Result<Customer>.Success(this);
    }

    /// <summary>
    /// Updates the corporate information (only for Corporate customers).
    /// </summary>
    public Result<Customer> UpdateCorporateInfo(string companyName, string taxNumber)
    {
        if (CustomerType != CustomerType.Corporate)
            return Result<Customer>.Failure("Corporate information can only be updated for corporate customers");

        if (string.IsNullOrWhiteSpace(companyName))
            return Result<Customer>.Failure("Company name is required");

        if (string.IsNullOrWhiteSpace(taxNumber))
            return Result<Customer>.Failure("Tax number is required");

        CompanyName = companyName.Trim();
        TaxNumber = taxNumber.Trim();
        UpdatedAt = DateTime.UtcNow;
        return Result<Customer>.Success(this);
    }

    /// <summary>
    /// Returns the full display name of the customer.
    /// </summary>
    public string GetDisplayName() =>
        CustomerType == CustomerType.Corporate && !string.IsNullOrWhiteSpace(CompanyName)
            ? $"{CompanyName} ({FirstName} {LastName})"
            : $"{FirstName} {LastName}";
}
