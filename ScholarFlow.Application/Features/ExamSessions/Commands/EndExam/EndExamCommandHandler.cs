using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.EndExam;

public class EndExamCommandHandler : IRequestHandler<EndExamCommand, Result<ExamSessionDto>>
{
    private readonly IApplicationDbContext _context;

    public EndExamCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ExamSessionDto>> Handle(EndExamCommand request, CancellationToken cancellationToken)
    {
        // Get exam session with responses
        var examSession = await _context.ExamSessions
            .Include(es => es.User)
            .Include(es => es.Paper)
                .ThenInclude(p => p.Subject)
            .Include(es => es.Paper)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.Options)
            .Include(es => es.UserResponses)
                .ThenInclude(ur => ur.SelectedOption)
            .FirstOrDefaultAsync(es => es.Id == request.SessionId, cancellationToken);

        if (examSession == null)
        {
            return Result<ExamSessionDto>.Failure("Exam session not found");
        }

        // Verify ownership
        if (examSession.UserId != request.UserId)
        {
            return Result<ExamSessionDto>.Failure("You are not authorized to end this exam session");
        }

        // Check if already completed
        if (examSession.Status != ExamSessionStatus.InProgress)
        {
            return Result<ExamSessionDto>.Failure("Exam session is not in progress");
        }

        // Calculate score (auto-grade MCQ questions)
        int totalQuestions = examSession.Paper.Questions.Count;
        int correctAnswers = 0;

        foreach (var response in examSession.UserResponses)
        {
            if (response.SelectedOption != null && response.SelectedOption.IsCorrect)
            {
                correctAnswers++;
            }
        }

        decimal finalScore = totalQuestions > 0 
            ? (decimal)correctAnswers / totalQuestions * 100 
            : 0;

        // Update exam session
        examSession.EndTime = DateTime.UtcNow;
        examSession.FinalScore = Math.Round(finalScore, 2);
        examSession.Status = ExamSessionStatus.Completed;

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new ExamSessionDto
        {
            Id = examSession.Id,
            UserId = examSession.UserId,
            UserName = examSession.User?.UserName ?? "",
            PaperId = examSession.PaperId,
            PaperTitle = $"{examSession.Paper?.Subject?.Name} - {examSession.Paper?.Year} ({examSession.Paper?.Type})",
            StartTime = examSession.StartTime,
            EndTime = examSession.EndTime,
            FinalScore = examSession.FinalScore,
            Status = examSession.Status,
            StatusName = examSession.Status.ToString(),
            Duration = examSession.Duration,
            TotalQuestions = totalQuestions,
            AnsweredQuestions = examSession.UserResponses.Count
        };

        return Result<ExamSessionDto>.Success(dto);
    }
}
