using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.ExamSessions.Queries.GetExamSessionReview;

public class GetExamSessionReviewQueryHandler : IRequestHandler<GetExamSessionReviewQuery, Result<List<ExamSessionReviewItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetExamSessionReviewQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ExamSessionReviewItemDto>>> Handle(GetExamSessionReviewQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.ExamSessions
            .Include(es => es.UserResponses)
                .ThenInclude(ur => ur.Question)
                    .ThenInclude(q => q.Options)
            .Include(es => es.UserResponses)
                .ThenInclude(ur => ur.Question)
                    .ThenInclude(q => q.Explanations)
                        .ThenInclude(e => e.Sections)
            .Include(es => es.UserResponses)
                .ThenInclude(ur => ur.SelectedOption)
            .FirstOrDefaultAsync(es => es.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            return Result<List<ExamSessionReviewItemDto>>.Failure("Exam session not found");
        }

        if (session.UserId != request.UserId)
        {
            return Result<List<ExamSessionReviewItemDto>>.Failure("You are not authorized to view this review");
        }

        var wrongItems = session.UserResponses
            .Where(ur => !ur.IsCorrect)
            .OrderBy(ur => ur.Question.OrderIndex)
            .ThenBy(ur => ur.QuestionId)
            .Select(ur =>
            {
                var correctOption = ur.Question.Options.FirstOrDefault(o => o.IsCorrect);
                var explanation = ur.Question.Explanations
                    .SelectMany(e => e.Sections)
                    .Where(s => s.Type == ContentType.Text || s.Type == ContentType.Formula || s.Type == ContentType.Code)
                    .OrderBy(s => s.OrderIndex)
                    .Select(s => s.Content)
                    .FirstOrDefault()
                    ?? ur.Question.Explanations.Select(e => e.Title).FirstOrDefault();

                return new ExamSessionReviewItemDto
                {
                    QuestionId = ur.QuestionId,
                    OrderIndex = ur.Question.OrderIndex,
                    QuestionText = ur.Question.QuestionText,
                    QuestionImageUrl = ur.Question.QuestionImageUrl,
                    SelectedOptionId = ur.SelectedOptionId,
                    SelectedOptionText = ur.SelectedOption?.OptionText ?? string.Empty,
                    CorrectOptionId = correctOption?.Id ?? Guid.Empty,
                    CorrectOptionText = correctOption?.OptionText ?? string.Empty,
                    Explanation = explanation
                };
            })
            .ToList();

        return Result<List<ExamSessionReviewItemDto>>.Success(wrongItems);
    }
}
