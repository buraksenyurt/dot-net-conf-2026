namespace VehicleInventory.Application.DTOs;

/// <summary>
/// Vehicle data transfer object.
/// </summary>
public record VehicleDto(
    Guid Id,
    string VIN,
    string Brand,
    string Model,
    int Year,
    string EngineType,
    int Mileage,
    string Color,
    decimal PurchaseAmount,
    string PurchaseCurrency,
    decimal SuggestedAmount,
    string SuggestedCurrency,
    string TransmissionType,
    decimal FuelConsumption,
    int EngineCapacity,
    List<string> Features,
    string Status,
    DateTime CreatedAt
);

/// <summary>
/// Paged result wrapper.
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
