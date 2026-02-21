using MediatR;
using Microsoft.Extensions.Logging;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Enums;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Handler for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(ICustomerRepository repository, ILogger<CreateCustomerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer. Email: {Email}, Type: {Type}", request.Email, request.CustomerType);

        // Check for duplicate e-mail
        if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _logger.LogWarning("Customer with email {Email} already exists", request.Email);
            return Result<Guid>.Failure($"A customer with email '{request.Email}' already exists");
        }

        // Create Email value object
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<Guid>.Failure(emailResult.Error);

        // Parse customer type
        if (!Enum.TryParse<CustomerType>(request.CustomerType, ignoreCase: true, out var customerType))
            return Result<Guid>.Failure($"Invalid customer type: {request.CustomerType}");

        // Create Customer entity using the appropriate factory method
        Result<Customer> customerResult = customerType switch
        {
            CustomerType.Individual => Customer.CreateIndividual(
                request.FirstName,
                request.LastName,
                emailResult.Value!,
                request.Phone),

            CustomerType.Corporate => Customer.CreateCorporate(
                request.FirstName,
                request.LastName,
                emailResult.Value!,
                request.Phone,
                request.CompanyName ?? string.Empty,
                request.TaxNumber ?? string.Empty),

            _ => Result<Customer>.Failure($"Unsupported customer type: {customerType}")
        };

        if (customerResult.IsFailure)
        {
            _logger.LogError("Failed to create customer: {Error}", customerResult.Error);
            return Result<Guid>.Failure(customerResult.Error);
        }

        var saved = await _repository.AddAsync(customerResult.Value!, cancellationToken);
        _logger.LogInformation("Customer created with Id: {Id}", saved.Id);
        return Result<Guid>.Success(saved.Id);
    }
}
