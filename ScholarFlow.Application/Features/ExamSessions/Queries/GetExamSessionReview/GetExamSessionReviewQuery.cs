using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.ExamSessions.Queries.GetExamSessionReview;

public class GetExamSessionReviewQuery : IRequest<Result<List<ExamSessionReviewItemDto>>>
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
}
