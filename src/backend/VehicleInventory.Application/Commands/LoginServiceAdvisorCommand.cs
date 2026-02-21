using MediatR;
using VehicleInventory.Application.Abstractions;
using VehicleInventory.Application.DTOs;
using VehicleInventory.Domain.Common;
using VehicleInventory.Domain.Interfaces;

namespace VehicleInventory.Application.Commands;

/// <summary>
/// Command: service advisor login with email + password.
/// Returns the advisor DTO on success, Failure on bad credentials.
/// </summary>
public record LoginServiceAdvisorCommand(
    string Email,
    string Password
) : IRequest<Result<ServiceAdvisorDto>>;

public class LoginServiceAdvisorCommandHandler
    : IRequestHandler<LoginServiceAdvisorCommand, Result<ServiceAdvisorDto>>
{
    private readonly IServiceAdvisorRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginServiceAdvisorCommandHandler(
        IServiceAdvisorRepository repository,
        IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<ServiceAdvisorDto>> Handle(
        LoginServiceAdvisorCommand request,
        CancellationToken cancellationToken)
    {
        var advisor = await _repository.GetByEmailAsync(
            request.Email.Trim().ToLowerInvariant(),
            cancellationToken);

        // Return the same generic error to avoid user enumeration
        if (advisor is null || !advisor.IsActive)
            return Result<ServiceAdvisorDto>.Failure("E-posta veya şifre hatalı");

        if (!_passwordHasher.Verify(request.Password, advisor.PasswordHash))
            return Result<ServiceAdvisorDto>.Failure("E-posta veya şifre hatalı");

        return Result<ServiceAdvisorDto>.Success(new ServiceAdvisorDto(
            advisor.Id,
            advisor.FirstName,
            advisor.LastName,
            advisor.Email.Value,
            advisor.Department,
            advisor.IsActive));
    }
}
