using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Queries;

namespace VehicleInventory.API.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Creates a new customer.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("already exists"))
                return Conflict(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>Returns a paginated and filtered list of customers.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? customerType = null)
    {
        var query = new GetCustomersQuery(page, pageSize, search, customerType);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>Returns a single customer by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result is null)
            return NotFound(new { error = $"Customer with id '{id}' was not found" });

        return Ok(result);
    }
}
