using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Papers.Queries.GetPaperWithQuestions;

public class GetPaperWithQuestionsQueryHandler : IRequestHandler<GetPaperWithQuestionsQuery, Result<PaperDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPaperWithQuestionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaperDetailDto>> Handle(GetPaperWithQuestionsQuery request, CancellationToken cancellationToken)
    {
        var paper = await _context.Papers
            .Include(p => p.Subject)
            .Include(p => p.Creator)
            .Include(p => p.Questions)
                .ThenInclude(q => q.SubTopic)
                    .ThenInclude(st => st.Topic)
                        .ThenInclude(t => t.Subject)
            .Include(p => p.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (paper == null)
        {
            return Result<PaperDetailDto>.Failure("Paper not found");
        }

        var dto = new PaperDetailDto
        {
            Id = paper.Id,
            SubjectId = paper.SubjectId,
            SubjectName = paper.Subject.Name,
            Year = paper.Year,
            Type = paper.Type.ToString(),
            Title = paper.Title,
            TimeLimit = paper.TimeLimit,
            CreatedByTeacher = paper.CreatedByTeacher,
            CreatedByTeacherName = paper.Creator.UserName ?? "",
            QuestionCount = paper.Questions.Count,
            Questions = paper.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                PaperId = q.PaperId,
                SubTopicId = q.SubTopicId,
                SubTopicName = q.SubTopic?.SubTopicName ?? "", 
                TopicId = q.SubTopic?.TopicId ?? Guid.Empty,
                TopicName = q.SubTopic?.Topic?.TopicName ?? "",
                SubjectId = q.SubTopic?.Topic?.SubjectId ?? Guid.Empty,
                SubjectName = q.SubTopic?.Topic?.Subject?.Name ?? "", 
                QuestionText = q.QuestionText,
                QuestionImageUrl = q.QuestionImageUrl,
                Difficulty = q.Difficulty,
                Marks = q.Marks,
                OrderIndex = q.OrderIndex,
                Options = q.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
                    OrderIndex = o.OrderIndex
                }).OrderBy(o => o.OrderIndex).ToList()
            }).OrderBy(q => q.OrderIndex).ToList()
        };

        return Result<PaperDetailDto>.Success(dto);
    }
}
