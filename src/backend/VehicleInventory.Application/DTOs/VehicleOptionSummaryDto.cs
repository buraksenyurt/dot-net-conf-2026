namespace VehicleInventory.Application.DTOs;

/// <summary>
/// DTO for the vehicle option summary list — US-007.
/// The <see cref="Status"/> field reflects the effective status at query time:
/// Active records whose <c>ExpiresAt</c> has already passed are surfaced as Expired
/// without touching the database row (BR-2).
/// </summary>
public record VehicleOptionSummaryDto(
    Guid Id,
    Guid VehicleId,
    string VehicleDisplayName,
    string VehicleVIN,
    Guid CustomerId,
    string CustomerDisplayName,
    Guid? ServiceAdvisorId,
    string? ServiceAdvisorDisplayName,
    DateTime ExpiresAt,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    string? Notes,
    int Status,
    bool IsExpired,
    DateTime CreatedAt
);
