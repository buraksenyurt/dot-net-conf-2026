using MediatR;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command to create a vehicle purchase option (reservation) for a customer.
/// ValidityDays: 1–30. OptionFeeAmount: >= 0.
/// ServiceAdvisorId is optional — set when the option is created by a logged-in advisor.
/// </summary>
public record CreateVehicleOptionCommand(
    Guid VehicleId,
    Guid CustomerId,
    int ValidityDays,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    Guid? ServiceAdvisorId = null,
    string? Notes = null
) : IRequest<Result<Guid>>;
