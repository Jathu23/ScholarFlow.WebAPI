using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Features.Papers.Commands.CreatePaper;
using ScholarFlow.Application.Features.Papers.Commands.DeletePaper;
using ScholarFlow.Application.Features.Papers.Commands.UpdatePaper;
using ScholarFlow.Application.Features.Papers.Queries.GetPaperById;
using ScholarFlow.Application.Features.Papers.Queries.GetPapers;
using ScholarFlow.Application.Features.Papers.Queries.GetPaperWithQuestions;
using ScholarFlow.Application.Features.Questions.Commands.BulkCreateQuestions;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Papers API Controller
/// </summary>
[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public PapersController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
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
        [FromQuery] bool adminCreatedOnly,
        [FromQuery] string? teacherCode,
        CancellationToken cancellationToken)
    {
        var query = new GetPapersQuery 
        { 
            SubjectId = subjectId,
            Year = year,
            Type = type,
            AdminCreatedOnly = adminCreatedOnly
        };

        if (!string.IsNullOrWhiteSpace(teacherCode))
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var studentUserId))
            {
                return Unauthorized(new { error = "Login required to access teacher guided papers." });
            }

            var isStudent = User.Claims.Any(c =>
                (c.Type == "role" || c.Type.EndsWith("/role"))
                && string.Equals(c.Value, "Student", StringComparison.OrdinalIgnoreCase));

            if (!isStudent)
            {
                return Forbid();
            }

            var normalizedCode = teacherCode.Trim().ToUpperInvariant();
            var teacherProfile = await _context.TeacherProfiles
                .FirstOrDefaultAsync(
                    t => t.TeacherCode != null
                         && t.TeacherCode.ToUpper() == normalizedCode
                         && t.Status == TeacherRegistrationStatus.Accepted,
                    cancellationToken);

            if (teacherProfile == null)
            {
                return BadRequest(new { error = "Teacher ID is invalid or not approved yet." });
            }

            if (subjectId.HasValue && teacherProfile.SubjectId != subjectId.Value)
            {
                return BadRequest(new { error = "This teacher ID is not linked to the selected subject." });
            }

            var hasApprovedAccess = await _context.StudentTeacherConnections.AnyAsync(
                x => x.StudentUserId == studentUserId
                     && x.TeacherUserId == teacherProfile.UserId
                     && x.Status == StudentTeacherConnectionStatus.Approved
                     && (!subjectId.HasValue || x.SubjectId == subjectId.Value),
                cancellationToken);

            if (!hasApprovedAccess)
            {
                return StatusCode(403, new { error = "Teacher has not approved your access request for this subject." });
            }

            query.CreatedByTeacherId = teacherProfile.UserId;
        }
        
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
    //[Authorize(Roles = "Teacher,Admin")]
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
    /// Update an existing paper
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaperCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        command.Id = id;
        command.UpdatedByTeacher = userId;

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
    /// <summary>
    /// Get paper by ID with questions and options
    /// </summary>
    [HttpGet("{id}/questions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdWithQuestions(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaperWithQuestionsQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.IsSuccess)
        {
             return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data); 
    }
    /// <summary>
    /// Bulk create questions for a paper
    /// </summary>
    [HttpPost("{id}/questions/bulk")]
    //[Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> BulkCreateQuestions(Guid id, [FromBody] List<CreateQuestionDto> questions, CancellationToken cancellationToken)
    {
        var command = new BulkCreateQuestionsCommand 
        { 
            PaperId = id, 
            Questions = questions 
        };
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(new { message = "Questions created successfully" }) 
            : BadRequest(new { error = result.ErrorMessage });
    }
}
