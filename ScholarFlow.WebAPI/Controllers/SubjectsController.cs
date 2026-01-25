using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Subjects.Commands.CreateSubject;
using ScholarFlow.Application.Features.Subjects.Commands.DeleteSubject;
using ScholarFlow.Application.Features.Subjects.Commands.UpdateSubject;
using ScholarFlow.Application.Features.Subjects.Queries.GetSubjectById;
using ScholarFlow.Application.Features.Subjects.Queries.GetSubjects;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Subjects API Controller
/// </summary>
[ApiController]
[Route("api/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all subjects (optionally filter by stream)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] Guid? streamId, CancellationToken cancellationToken)
    {
        var query = new GetSubjectsQuery { StreamId = streamId };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Get subject by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSubjectByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : NotFound(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Create a new subject
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Update a subject
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Delete a subject (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSubjectCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(new { message = "Subject deleted successfully" }) 
            : NotFound(new { error = result.ErrorMessage });
    }
}
