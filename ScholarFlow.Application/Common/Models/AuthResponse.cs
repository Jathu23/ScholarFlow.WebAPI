namespace ScholarFlow.Application.Common.Models;

/// <summary>
/// JWT authentication response - Token contains all user information
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
