using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Subjects.Commands.CreateSubject;
using ScholarFlow.Application.Features.Subjects.Commands.DeleteSubject;
using ScholarFlow.Application.Features.Subjects.Commands.UpdateSubject;
using ScholarFlow.Application.Features.Subjects.Queries.GetSubjectById;
using ScholarFlow.Application.Features.Subjects.Queries.GetSubjects;
using ScholarFlow.Application.Features.Subjects.Queries.GetAcademicStructure;
using ScholarFlow.Application.Features.Subjects.Commands.BulkCreateAcademicStructure;
using ScholarFlow.Application.DTOs;

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
    /// <summary>
    /// Get full academic structure (Streams -> Subjects -> Topics -> SubTopics)
    /// </summary>
    [HttpGet("structure")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStructure(CancellationToken cancellationToken)
    {
        var query = new GetAcademicStructureQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }
    /// <summary>
    /// Bulk create/update academic structure
    /// </summary>
    [HttpPost("structure")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkCreateStructure([FromBody] List<CreateStreamDto> streams, CancellationToken cancellationToken)
    {
        var command = new BulkCreateAcademicStructureCommand { Streams = streams };
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(new { message = "Academic structure synced successfully" }) 
            : BadRequest(new { error = result.ErrorMessage });
    }
}
