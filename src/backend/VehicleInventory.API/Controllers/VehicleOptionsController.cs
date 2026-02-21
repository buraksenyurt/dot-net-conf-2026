using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Queries;

namespace VehicleInventory.API.Controllers;

/// <summary>
/// Vehicle Options API — US-003: Araç Opsiyonlama
/// </summary>
[ApiController]
[Route("api/vehicle-options")]
public class VehicleOptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehicleOptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a vehicle purchase option for a customer.
    /// On success the vehicle status is set to Reserved.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleOptionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = command.VehicleId }, new { id = result.Value });
    }

    /// <summary>
    /// Cancels an active vehicle option.
    /// On success the vehicle status is reverted to OnSale.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelVehicleOptionCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Option cancelled successfully" });
    }

    /// <summary>
    /// Returns all options (history) for a specific vehicle.
    /// </summary>
    [HttpGet("vehicle/{vehicleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVehicleOptionsByVehicleQuery(vehicleId), cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Returns all options for a specific customer.
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVehicleOptionsByCustomerQuery(customerId), cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}
