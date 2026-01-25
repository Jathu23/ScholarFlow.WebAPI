using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.SubmitAnswer;

/// <summary>
/// Command to submit an answer during an exam session
/// </summary>
public class SubmitAnswerCommand : IRequest<Result<bool>>
{
    public Guid SessionId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
    public Guid UserId { get; set; } // Will be set from JWT in controller
}
