using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.ExamSessions.Queries.GetMyExamSessions;

public class GetMyExamSessionsQueryHandler : IRequestHandler<GetMyExamSessionsQuery, Result<List<ExamSessionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMyExamSessionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ExamSessionDto>>> Handle(GetMyExamSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _context.ExamSessions
            .Include(es => es.User)
            .Include(es => es.Paper)
                .ThenInclude(p => p.Subject)
            .Include(es => es.Paper)
                .ThenInclude(p => p.Questions)
            .Include(es => es.UserResponses)
            .Where(es => es.UserId == request.UserId)
            .OrderByDescending(es => es.StartTime)
            .ToListAsync(cancellationToken);

        var dtos = sessions.Select(es => new ExamSessionDto
        {
            Id = es.Id,
            UserId = es.UserId,
            UserName = es.User?.UserName ?? "",
            PaperId = es.PaperId,
            PaperTitle = $"{es.Paper?.Subject?.Name} - {es.Paper?.Year} ({es.Paper?.Type})",
            StartTime = es.StartTime,
            EndTime = es.EndTime,
            FinalScore = es.FinalScore,
            Status = es.Status,
            StatusName = es.Status.ToString(),
            Duration = es.Duration,
            TotalQuestions = es.Paper?.Questions.Count ?? 0,
            AnsweredQuestions = es.UserResponses.Count
        }).ToList();

        return Result<List<ExamSessionDto>>.Success(dtos);
    }
}
