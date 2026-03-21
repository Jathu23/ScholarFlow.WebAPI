using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Common.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(
        string email,
        string password,
        string role,
        string? fullName = null,
        Guid? subjectId = null,
        string? qualification = null,
        string? phoneNumber = null,
        string? bio = null);
    Task<Result<AuthResponse>> LoginAsync(string email, string password);
}
