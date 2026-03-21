using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Application.Features.Questions.Commands.CreateQuestion;
using ScholarFlow.Application.Features.Questions.Commands.DeleteQuestion;
using ScholarFlow.Application.Features.Questions.Commands.UpdateQuestion;
using ScholarFlow.Application.Features.Questions.Queries.GetQuestionsByPaper;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Questions API Controller
/// </summary>
[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public QuestionsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
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
    /// Get random topic questions for unit practice.
    /// </summary>
    [HttpGet("topic/{topicId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTopic(
        Guid topicId,
        [FromQuery] int limit = 20,
        [FromQuery] string? questionIds = null,
        CancellationToken cancellationToken = default)
    {
        var requestedIds = (questionIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(id => Guid.TryParse(id, out var parsed) ? parsed : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var effectiveLimit = requestedIds.Count > 0 ? requestedIds.Count : Math.Clamp(limit, 1, 50);

        var baseQuery = _context.Questions
            .Include(q => q.SubTopic)
            .Include(q => q.Options)
            .Include(q => q.Explanations)
                .ThenInclude(e => e.Sections)
            .Include(q => q.Paper)
            .Where(q =>
                !q.IsDeleted
                && q.SubTopic.TopicId == topicId
                && (q.Paper.Type == PaperType.PastPaper || q.Paper.Type == PaperType.ModelPaper)
                && (
                    !_context.TeacherProfiles.Any(tp => tp.UserId == q.Paper.CreatedByTeacher)
                    || _context.TeacherProfiles.Any(tp => tp.UserId == q.Paper.CreatedByTeacher && tp.Status == TeacherRegistrationStatus.Accepted)
                ));

        if (requestedIds.Count > 0)
        {
            baseQuery = baseQuery.Where(q => requestedIds.Contains(q.Id));
        }

        var selectedQuestions = requestedIds.Count > 0
            ? await baseQuery.ToListAsync(cancellationToken)
            : await baseQuery
                .OrderBy(q => Guid.NewGuid())
                .Take(effectiveLimit)
                .ToListAsync(cancellationToken);

        if (selectedQuestions.Count == 0)
        {
            // Fallback for legacy datasets where teacher approval status data may be incomplete.
            var fallbackQuery = _context.Questions
                .Include(q => q.SubTopic)
                .Include(q => q.Options)
                .Include(q => q.Explanations)
                    .ThenInclude(e => e.Sections)
                .Include(q => q.Paper)
                .Where(q =>
                    !q.IsDeleted
                    && q.SubTopic.TopicId == topicId
                    && (q.Paper.Type == PaperType.PastPaper || q.Paper.Type == PaperType.ModelPaper));

            if (requestedIds.Count > 0)
            {
                fallbackQuery = fallbackQuery.Where(q => requestedIds.Contains(q.Id));
            }

            selectedQuestions = requestedIds.Count > 0
                ? await fallbackQuery.ToListAsync(cancellationToken)
                : await fallbackQuery
                    .OrderBy(q => Guid.NewGuid())
                    .Take(effectiveLimit)
                    .ToListAsync(cancellationToken);
        }

        var dtos = selectedQuestions.Select(q => new QuestionDto
        {
            Id = q.Id,
            PaperId = q.PaperId,
            SubTopicId = q.SubTopicId,
            SubTopicName = q.SubTopic?.SubTopicName ?? string.Empty,
            QuestionText = q.QuestionText,
            QuestionImageUrl = q.QuestionImageUrl,
            Explanation = q.Explanations
                .SelectMany(e => e.Sections)
                .Where(s => s.Type == ContentType.Text || s.Type == ContentType.Formula || s.Type == ContentType.Code)
                .OrderBy(s => s.OrderIndex)
                .Select(s => s.Content)
                .FirstOrDefault()
                ?? q.Explanations.Select(e => e.Title).FirstOrDefault(),
            Difficulty = q.Difficulty,
            Marks = q.Marks,
            OrderIndex = q.OrderIndex,
            Options = q.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                ContentType = o.ContentType,
                ImageUrl = o.ImageUrl,
                Equation = o.Equation,
                IsCorrect = o.IsCorrect,
                OrderIndex = o.OrderIndex
            }).OrderBy(o => o.OrderIndex).ToList()
        }).ToList();

        if (requestedIds.Count > 0)
        {
            var orderLookup = requestedIds
                .Select((id, index) => new { id, index })
                .ToDictionary(x => x.id, x => x.index);

            dtos = dtos.OrderBy(d => orderLookup.TryGetValue(d.Id, out var idx) ? idx : int.MaxValue).ToList();
        }

        return Ok(dtos);
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
    /// Update an existing question
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;

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
