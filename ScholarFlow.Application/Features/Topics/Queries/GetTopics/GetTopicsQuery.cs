using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Topics.Queries.GetTopics;

/// <summary>
/// Query to get topics with optional subject filter
/// </summary>
public class GetTopicsQuery : IRequest<Result<List<TopicDto>>>
{
    public Guid? SubjectId { get; set; }
}
