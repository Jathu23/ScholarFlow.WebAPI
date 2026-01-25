using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.ExamSessions.Commands.EndExam;
using ScholarFlow.Application.Features.ExamSessions.Commands.StartExam;
using ScholarFlow.Application.Features.ExamSessions.Commands.SubmitAnswer;
using ScholarFlow.Application.Features.ExamSessions.Queries.GetMyExamSessions;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// ExamSessions API Controller
/// </summary>
[ApiController]
[Route("api/examsessions")]
[Authorize(Roles = "Student")]
public class ExamSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Start a new exam session
    /// </summary>
    [HttpPost("start")]
    public async Task<IActionResult> StartExam([FromBody] StartExamCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        command.UserId = userId;

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// End an exam session and calculate score
    /// </summary>
    [HttpPost("{sessionId}/end")]
    public async Task<IActionResult> EndExam(Guid sessionId, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        var command = new EndExamCommand 
        { 
            SessionId = sessionId,
            UserId = userId 
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Get my exam session history
    /// </summary>
    [HttpGet("my-sessions")]
    public async Task<IActionResult> GetMySessions(CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        var query = new GetMyExamSessionsQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Submit an answer for a question during exam
    /// </summary>
    [HttpPost("{sessionId}/submit-answer")]
    public async Task<IActionResult> SubmitAnswer(Guid sessionId, [FromBody] SubmitAnswerRequest request, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        var command = new SubmitAnswerCommand
        {
            SessionId = sessionId,
            QuestionId = request.QuestionId,
            SelectedOptionId = request.SelectedOptionId,
            UserId = userId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(new { message = "Answer submitted successfully" }) 
            : BadRequest(new { error = result.ErrorMessage });
    }
}

/// <summary>
/// Request model for submitting an answer
/// </summary>
public class SubmitAnswerRequest
{
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}

