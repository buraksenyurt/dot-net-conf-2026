namespace VehicleInventory.Application.DTOs;

/// <summary>
/// Data transfer object for ServiceAdvisor query/login results.
/// </summary>
public record ServiceAdvisorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    bool IsActive
);
