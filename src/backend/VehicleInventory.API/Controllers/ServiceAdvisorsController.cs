using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleInventory.Application.Commands;
using VehicleInventory.Application.Queries;

namespace VehicleInventory.API.Controllers;

[ApiController]
[Route("api/service-advisors")]
[Produces("application/json")]
public class ServiceAdvisorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceAdvisorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Service advisor login — returns advisor info on success.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginServiceAdvisorCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Returns all vehicle options assigned to the given service advisor.
    /// Used to populate the advisor's personal dashboard.
    /// </summary>
    [HttpGet("{id:guid}/dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard(Guid id)
    {
        var result = await _mediator.Send(new GetAdvisorDashboardQuery(id));

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }
}
