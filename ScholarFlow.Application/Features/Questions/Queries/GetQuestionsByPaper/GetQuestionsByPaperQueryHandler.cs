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
            .Where(q => q.PaperId == request.PaperId)
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
            Difficulty = q.Difficulty,
            Options = q.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect
            }).ToList()
        }).ToList();

        return Result<List<QuestionDto>>.Success(dtos);
    }
}
