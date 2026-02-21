namespace VehicleInventory.Application.DTOs;

/// <summary>
/// Customer data transfer object.
/// </summary>
public record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CustomerType,
    string? CompanyName,
    string? TaxNumber,
    string DisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
