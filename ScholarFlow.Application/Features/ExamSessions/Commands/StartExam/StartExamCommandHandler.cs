using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.StartExam;

public class StartExamCommandHandler : IRequestHandler<StartExamCommand, Result<ExamSessionDto>>
{
    private readonly IApplicationDbContext _context;

    public StartExamCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ExamSessionDto>> Handle(StartExamCommand request, CancellationToken cancellationToken)
    {
        // Check if paper exists
        var paper = await _context.Papers
            .Include(p => p.Subject)
            .Include(p => p.Questions)
            .FirstOrDefaultAsync(p => p.Id == request.PaperId, cancellationToken);

        if (paper == null)
        {
            return Result<ExamSessionDto>.Failure("Paper not found");
        }

        // Check if user exists
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<ExamSessionDto>.Failure("User not found");
        }

        // Check if user already has an active session for this paper
        var activeSession = await _context.ExamSessions
            .FirstOrDefaultAsync(es => es.UserId == request.UserId && 
                                      es.PaperId == request.PaperId &&
                                      es.Status == ExamSessionStatus.InProgress, 
                                cancellationToken);

        if (activeSession != null)
        {
            return Result<ExamSessionDto>.Failure("You already have an active exam session for this paper");
        }

        // Create exam session
        var examSession = new ExamSession
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PaperId = request.PaperId,
            StartTime = DateTime.UtcNow,
            Status = ExamSessionStatus.InProgress,
            FinalScore = 0
        };

        _context.ExamSessions.Add(examSession);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new ExamSessionDto
        {
            Id = examSession.Id,
            UserId = examSession.UserId,
            UserName = user.UserName ?? "",
            PaperId = examSession.PaperId,
            PaperTitle = $"{paper.Subject?.Name} - {paper.Year} ({paper.Type})",
            StartTime = examSession.StartTime,
            EndTime = examSession.EndTime,
            FinalScore = examSession.FinalScore,
            Status = examSession.Status,
            StatusName = examSession.Status.ToString(),
            Duration = examSession.Duration,
            TotalQuestions = paper.Questions.Count,
            AnsweredQuestions = 0
        };

        return Result<ExamSessionDto>.Success(dto);
    }
}
