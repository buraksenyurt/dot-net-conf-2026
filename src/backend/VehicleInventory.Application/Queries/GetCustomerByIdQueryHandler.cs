using MediatR;
using Microsoft.Extensions.Logging;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Handler for GetCustomerByIdQuery.
/// </summary>
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(ICustomerRepository repository, ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching customer by Id: {Id}", request.Id);

        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning("Customer with Id {Id} not found", request.Id);
            return null;
        }

        return new CustomerDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email.Value,
            customer.Phone,
            customer.CustomerType.ToString(),
            customer.CompanyName,
            customer.TaxNumber,
            customer.GetDisplayName(),
            customer.CreatedAt,
            customer.UpdatedAt
        );
    }
}
