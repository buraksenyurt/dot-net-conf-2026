namespace VehicleInventory.Application.DTOs;

/// <summary>
/// Data transfer object for VehicleOption query results.
/// </summary>
public record VehicleOptionDto(
    Guid Id,
    Guid VehicleId,
    string VehicleDisplayName,
    string VehicleVIN,
    Guid CustomerId,
    string CustomerDisplayName,
    DateTime ExpiresAt,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    string? Notes,
    string Status,
    bool IsExpired,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid? ServiceAdvisorId,
    string? ServiceAdvisorDisplayName
);
