using MediatR;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command to create a vehicle purchase option (reservation) for a customer.
/// ValidityDays: 1–30. OptionFeeAmount: >= 0.
/// </summary>
public record CreateVehicleOptionCommand(
    Guid VehicleId,
    Guid CustomerId,
    int ValidityDays,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    string? Notes = null
) : IRequest<Result<Guid>>;
