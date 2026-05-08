using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Queries;
using VehicleInventory.Domain.Enums;

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

    /// <summary>
    /// Returns a filterable and paginated summary of all vehicle options across all customers.
    /// US-007: Araç Opsiyonlama Özet Listesi.
    /// </summary>
    /// <param name="customerSearch">Case-insensitive partial match on customer first/last name.</param>
    /// <param name="vehicleSearch">Case-insensitive partial match on brand, model or VIN.</param>
    /// <param name="status">1=Active, 2=Expired, 3=Cancelled. Omit to return all.</param>
    /// <param name="createdFrom">Filter options created on or after this UTC date.</param>
    /// <param name="createdTo">Filter options created on or before this UTC date.</param>
    /// <param name="page">1-based page number. Default: 1.</param>
    /// <param name="pageSize">Records per page: 10, 20 or 50. Default: 20.</param>
    /// <param name="sortBy">expiresAt | createdAt | customerName. Default: expiresAt.</param>
    /// <param name="sortDirection">asc | desc. Default: asc.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? customerSearch,
        [FromQuery] string? vehicleSearch,
        [FromQuery] int? status,
        [FromQuery] DateTime? createdFrom,
        [FromQuery] DateTime? createdTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "expiresAt",
        [FromQuery] string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return BadRequest(new { error = "Page must be greater than 0." });

        if (pageSize is not (10 or 20 or 50))
            return BadRequest(new { error = "PageSize must be 10, 20 or 50." });

        VehicleOptionStatus? statusFilter = status.HasValue ? (VehicleOptionStatus)status.Value : null;

        var result = await _mediator.Send(
            new GetVehicleOptionSummaryQuery(
                customerSearch,
                vehicleSearch,
                statusFilter,
                createdFrom,
                createdTo,
                page,
                pageSize,
                sortBy,
                sortDirection),
            cancellationToken);

        return Ok(result);
    }
}
