using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.StartExam;

/// <summary>
/// Command to start an exam session
/// </summary>
public class StartExamCommand : IRequest<Result<ExamSessionDto>>
{
    public Guid PaperId { get; set; }
    public Guid? TopicId { get; set; }
    public int QuestionLimit { get; set; } = 20;
    public Guid UserId { get; set; } // Will be set from JWT in controller
}
