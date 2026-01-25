using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Papers.Commands.CreatePaper;
using ScholarFlow.Application.Features.Papers.Commands.DeletePaper;
using ScholarFlow.Application.Features.Papers.Queries.GetPaperById;
using ScholarFlow.Application.Features.Papers.Queries.GetPapers;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Papers API Controller
/// </summary>
[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PapersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all papers with optional filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? subjectId, 
        [FromQuery] int? year,
        [FromQuery] PaperType? type,
        CancellationToken cancellationToken)
    {
        var query = new GetPapersQuery 
        { 
            SubjectId = subjectId,
            Year = year,
            Type = type
        };
        
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Get paper by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaperByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : NotFound(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Create a new paper
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePaperCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Set creator
        command.CreatedByTeacher = userId;

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Delete a paper (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePaperCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(new { message = "Paper deleted successfully" }) 
            : NotFound(new { error = result.ErrorMessage });
    }
}
