using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Auth.Commands.Login;
using ScholarFlow.Application.Features.Auth.Commands.Register;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Authentication API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Debug endpoint to check token claims
    /// </summary>
    [HttpGet("debug-token")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult DebugToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userId = User.FindFirst("userId")?.Value;
        var role = User.FindFirst("role")?.Value;
        
        return Ok(new 
        { 
            allClaims = claims,
            extractedUserId = userId,
            extractedRole = role,
            isAuthenticated = User.Identity?.IsAuthenticated,
            userName = User.Identity?.Name
        });
    }

}
