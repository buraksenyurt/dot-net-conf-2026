using MediatR;
using Microsoft.Extensions.Logging;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Queries;

/// <summary>
/// Handler for GetCustomersQuery.
/// </summary>
public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(ICustomerRepository repository, ILogger<GetCustomersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching customers. Page: {Page}, PageSize: {PageSize}, Search: {Search}, Type: {Type}",
            request.Page, request.PageSize, request.SearchTerm, request.CustomerType);

        var (customers, totalCount) = await _repository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.CustomerType,
            cancellationToken);

        _logger.LogInformation("Found {Count} customers out of {TotalCount} total.", customers.Count(), totalCount);

        var dtos = customers.Select(c => new CustomerDto(
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email.Value,
            c.Phone,
            c.CustomerType.ToString(),
            c.CompanyName,
            c.TaxNumber,
            c.GetDisplayName(),
            c.CreatedAt,
            c.UpdatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PagedResult<CustomerDto>(dtos, request.Page, request.PageSize, totalCount, totalPages);
    }
}
