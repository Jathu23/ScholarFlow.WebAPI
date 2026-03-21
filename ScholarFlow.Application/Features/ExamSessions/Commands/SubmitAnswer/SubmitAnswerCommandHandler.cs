using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.SubmitAnswer;

public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public SubmitAnswerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        // Get exam session
        var session = await _context.ExamSessions
            .Include(es => es.Paper)
                .ThenInclude(p => p.Questions)
            .FirstOrDefaultAsync(es => es.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            return Result<bool>.Failure("Exam session not found");
        }

        // Verify ownership
        if (session.UserId != request.UserId)
        {
            return Result<bool>.Failure("You are not authorized to submit answers for this exam session");
        }

        // Check if session is still in progress
        if (session.Status != ExamSessionStatus.InProgress)
        {
            return Result<bool>.Failure("Exam session is not in progress");
        }

        // Verify question is allowed for this session (pre-seeded list for unit/full sessions).
        var questionExistsInSession = await _context.UserResponses
            .AnyAsync(
                ur => ur.SessionId == request.SessionId && ur.QuestionId == request.QuestionId,
                cancellationToken);

        var questionExistsInPaper = session.Paper.Questions.Any(q => q.Id == request.QuestionId);
        if (!questionExistsInSession && !questionExistsInPaper)
        {
            return Result<bool>.Failure("Question does not belong to this session");
        }

        // Verify option exists and belongs to the question
        var option = await _context.Options
            .FirstOrDefaultAsync(o => o.Id == request.SelectedOptionId && 
                                     o.QuestionId == request.QuestionId, 
                                cancellationToken);

        if (option == null)
        {
            return Result<bool>.Failure("Invalid option selected");
        }

        // Check if answer already exists
        var existingResponse = await _context.UserResponses
            .FirstOrDefaultAsync(ur => ur.SessionId == request.SessionId && 
                                      ur.QuestionId == request.QuestionId, 
                                cancellationToken);

        if (existingResponse != null)
        {
            // Update existing answer
            existingResponse.SelectedOptionId = request.SelectedOptionId;
            existingResponse.IsCorrect = option.IsCorrect;
            existingResponse.ResponseStatus = ResponseStatus.Answered;
        }
        else
        {
            // Create new response
            var response = new UserResponse
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                QuestionId = request.QuestionId,
                SelectedOptionId = request.SelectedOptionId,
                IsCorrect = option.IsCorrect,
                ResponseStatus = ResponseStatus.Answered,
                TimeSpentSeconds = 0 // Can be tracked on frontend
            };

            _context.UserResponses.Add(response);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
