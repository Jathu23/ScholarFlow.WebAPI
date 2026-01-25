using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.EndExam;

/// <summary>
/// Command to end an exam session and calculate score
/// </summary>
public class EndExamCommand : IRequest<Result<ExamSessionDto>>
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; } // Will be set from JWT in controller
}
