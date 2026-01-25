using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Students.Commands.CreateProfile;
using ScholarFlow.Application.Features.Students.Commands.UpdateProfile;
using ScholarFlow.Application.Features.Students.Queries.GetProfile;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Student Profile API Controller
/// </summary>
[ApiController]
[Route("api/students")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create student profile
    /// </summary>
    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateStudentProfileCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Set UserId in command
        command.UserId = userId;

        // Delegate to handler
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get current student's profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Create query with UserId
        var query = new GetStudentProfileQuery { UserId = userId };

        // Delegate to handler
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Update student profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Set UserId in command
        command.UserId = userId;

        // Delegate to handler
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}
