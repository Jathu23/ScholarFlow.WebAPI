using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command to register a new user
/// </summary>
public class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Default role
}
