using MediatR;
using ScholarFlow.Application.Common.Interfaces;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler for RegisterCommand
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.Role,
            request.FullName,
            request.SubjectId,
            request.Qualification,
            request.PhoneNumber,
            request.Bio);
    }
}
