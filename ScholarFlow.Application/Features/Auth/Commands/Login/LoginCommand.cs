using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to login a user
/// </summary>
public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
