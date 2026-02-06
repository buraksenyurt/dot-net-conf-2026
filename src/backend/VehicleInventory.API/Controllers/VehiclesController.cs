using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Queries;

namespace VehicleInventory.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateVehicle([FromBody] AddVehicleCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("already exists"))
                return Conflict(new { error = result.Error });
                
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetVehicle), new { id = result.Value }, new { id = result.Value });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        [FromQuery] string? brand = null, [FromQuery] string? status = null)
    {
        var query = new GetVehiclesQuery(page, pageSize, brand, status);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        // Placeholder - implement GetVehicleByIdQuery if needed
        return NotFound();
    }
}
