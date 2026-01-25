using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Questions.Commands.CreateQuestion;
using ScholarFlow.Application.Features.Questions.Commands.DeleteQuestion;
using ScholarFlow.Application.Features.Questions.Queries.GetQuestionsByPaper;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Questions API Controller
/// </summary>
[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all questions for a paper
    /// </summary>
    [HttpGet("paper/{paperId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPaper(Guid paperId, CancellationToken cancellationToken)
    {
        var query = new GetQuestionsByPaperQuery { PaperId = paperId };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Create a new question with options
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Delete a question (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteQuestionCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(new { message = "Question deleted successfully" }) 
            : NotFound(new { error = result.ErrorMessage });
    }
}
