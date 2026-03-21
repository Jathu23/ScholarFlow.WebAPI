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
        // Check if user exists
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<ExamSessionDto>.Failure("User not found");
        }

        var isUnitPractice = request.TopicId.HasValue;
        Paper? paper;
        List<Question> selectedQuestions;
        string paperTitle;

        if (isUnitPractice)
        {
            var topicId = request.TopicId!.Value;
            var questionLimit = request.QuestionLimit > 0 ? request.QuestionLimit : 20;

            var topic = await _context.Topics
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == topicId, cancellationToken);

            if (topic == null)
            {
                return Result<ExamSessionDto>.Failure("Topic not found");
            }

            var eligibleQuestions = await _context.Questions
                .Include(q => q.Paper)
                .Include(q => q.SubTopic)
                .Where(q =>
                    !q.IsDeleted
                    && q.SubTopic.TopicId == topicId
                    && (q.Paper.Type == PaperType.PastPaper || q.Paper.Type == PaperType.ModelPaper)
                    && (
                        !_context.TeacherProfiles.Any(tp => tp.UserId == q.Paper.CreatedByTeacher)
                        || _context.TeacherProfiles.Any(tp => tp.UserId == q.Paper.CreatedByTeacher && tp.Status == TeacherRegistrationStatus.Accepted)
                    ))
                .OrderBy(q => Guid.NewGuid())
                .Take(questionLimit)
                .ToListAsync(cancellationToken);

            if (eligibleQuestions.Count == 0)
            {
                // Fallback for legacy datasets where teacher approval status data may be incomplete.
                eligibleQuestions = await _context.Questions
                    .Include(q => q.Paper)
                    .Include(q => q.SubTopic)
                    .Where(q =>
                        !q.IsDeleted
                        && q.SubTopic.TopicId == topicId
                        && (q.Paper.Type == PaperType.PastPaper || q.Paper.Type == PaperType.ModelPaper))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(questionLimit)
                    .ToListAsync(cancellationToken);
            }

            if (eligibleQuestions.Count == 0)
            {
                return Result<ExamSessionDto>.Failure("No eligible questions found for this topic");
            }

            selectedQuestions = eligibleQuestions;

            var sessionPaperId = selectedQuestions[0].PaperId;
            paper = await _context.Papers
                .Include(p => p.Subject)
                .FirstOrDefaultAsync(p => p.Id == sessionPaperId, cancellationToken);

            if (paper == null)
            {
                return Result<ExamSessionDto>.Failure("Paper not found");
            }

            paperTitle = $"Unit Practice - {topic.TopicName}";
        }
        else
        {
            paper = await _context.Papers
                .Include(p => p.Subject)
                .Include(p => p.Questions)
                .FirstOrDefaultAsync(p => p.Id == request.PaperId, cancellationToken);

            if (paper == null)
            {
                return Result<ExamSessionDto>.Failure("Paper not found");
            }

            selectedQuestions = paper.Questions.ToList();
            paperTitle = $"{paper.Subject?.Name} - {paper.Year} ({paper.Type})";
        }

        var targetPaperId = paper.Id;

        // Check if user already has an active session for this paper
        var activeSession = await _context.ExamSessions
            .Include(es => es.UserResponses)
            .FirstOrDefaultAsync(es => es.UserId == request.UserId && 
                                      es.PaperId == targetPaperId &&
                                      es.Status == ExamSessionStatus.InProgress, 
                                cancellationToken);

        if (activeSession != null)
        {
            activeSession.Status = ExamSessionStatus.Abandoned;
            activeSession.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Create exam session
        var examSession = new ExamSession
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PaperId = targetPaperId,
            StartTime = DateTime.UtcNow,
            Status = ExamSessionStatus.InProgress,
            FinalScore = 0
        };

        _context.ExamSessions.Add(examSession);

        if (selectedQuestions.Count > 0)
        {
            var initialResponses = selectedQuestions.Select(q => new UserResponse
            {
                Id = Guid.NewGuid(),
                SessionId = examSession.Id,
                QuestionId = q.Id,
                SelectedOptionId = null,
                IsCorrect = false,
                ResponseStatus = ResponseStatus.Unvisited,
                TimeSpentSeconds = 0
            });

            _context.UserResponses.AddRange(initialResponses);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new ExamSessionDto
        {
            Id = examSession.Id,
            UserId = examSession.UserId,
            UserName = user.UserName ?? "",
            PaperId = examSession.PaperId,
            PaperTitle = paperTitle,
            StartTime = examSession.StartTime,
            EndTime = examSession.EndTime,
            FinalScore = examSession.FinalScore,
            Status = examSession.Status,
            StatusName = examSession.Status.ToString(),
            Duration = examSession.Duration,
            TotalQuestions = selectedQuestions.Count,
            AnsweredQuestions = 0,
            Responses = selectedQuestions.Select(q => new ExamSessionResponseDto
            {
                QuestionId = q.Id,
                SelectedOptionId = null
            }).ToList()
        };

        return Result<ExamSessionDto>.Success(dto);
    }
}
