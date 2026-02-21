using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Domain.Entities;

/// <summary>
/// ServiceAdvisor entity — represents an employee who manages vehicle options for customers.
/// Service advisors can log in and see the options they are responsible for.
/// </summary>
public sealed class ServiceAdvisor
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; }
    public string Department { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private ServiceAdvisor()
    {
        FirstName = null!;
        LastName = null!;
        PasswordHash = null!;
        Department = null!;
    }

    private ServiceAdvisor(
        Guid id,
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        string department)
    {
        Id = id;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email;
        PasswordHash = passwordHash;
        Department = department.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new ServiceAdvisor.
    /// </summary>
    public static Result<ServiceAdvisor> Create(
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        string department)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<ServiceAdvisor>.Failure("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<ServiceAdvisor>.Failure("Last name is required");

        if (string.IsNullOrWhiteSpace(department))
            return Result<ServiceAdvisor>.Failure("Department is required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<ServiceAdvisor>.Failure("Password hash is required");

        return Result<ServiceAdvisor>.Success(
            new ServiceAdvisor(Guid.NewGuid(), firstName, lastName, email, passwordHash, department));
    }

    public string GetDisplayName() => $"{FirstName} {LastName}";

    public void Deactivate()
    {
        IsActive = false;
    }
}
