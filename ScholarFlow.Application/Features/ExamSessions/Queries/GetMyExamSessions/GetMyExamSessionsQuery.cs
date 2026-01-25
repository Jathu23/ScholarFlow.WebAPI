using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.ExamSessions.Queries.GetMyExamSessions;

/// <summary>
/// Query to get current user's exam sessions
/// </summary>
public class GetMyExamSessionsQuery : IRequest<Result<List<ExamSessionDto>>>
{
    public Guid UserId { get; set; } // Will be set from JWT in controller
}
