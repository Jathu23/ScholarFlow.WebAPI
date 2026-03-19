using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Questions.Queries.GetQuestionsByPaper;

/// <summary>
/// Handler for GetQuestionsByPaperQuery
/// </summary>
public class GetQuestionsByPaperQueryHandler : IRequestHandler<GetQuestionsByPaperQuery, Result<List<QuestionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionsByPaperQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<QuestionDto>>> Handle(GetQuestionsByPaperQuery request, CancellationToken cancellationToken)
    {
        var questions = await _context.Questions
            .Include(q => q.SubTopic)
            .Include(q => q.Options)
            .Include(q => q.Explanations)
                .ThenInclude(e => e.Sections)
            .Where(q => q.PaperId == request.PaperId && !q.IsDeleted)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            PaperId = q.PaperId,
            SubTopicId = q.SubTopicId,
            SubTopicName = q.SubTopic?.SubTopicName ?? "",
            QuestionText = q.QuestionText,
            QuestionImageUrl = q.QuestionImageUrl,
            Explanation = q.Explanations
                .SelectMany(e => e.Sections)
                .Where(s => s.Type == Domain.Enums.ContentType.Text || s.Type == Domain.Enums.ContentType.Formula || s.Type == Domain.Enums.ContentType.Code)
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
        }).OrderBy(q => q.OrderIndex).ToList();

        return Result<List<QuestionDto>>.Success(dtos);
    }
}
