using MediatR;
using VehicleInventory.Domain.Common;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command to cancel an active vehicle option.
/// The vehicle will be reverted to OnSale status upon cancellation.
/// </summary>
public record CancelVehicleOptionCommand(Guid OptionId) : IRequest<Result<bool>>;
