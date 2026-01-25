using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Topics.Queries.GetTopicById;

/// <summary>
/// Query to get topic by ID
/// </summary>
public class GetTopicByIdQuery : IRequest<Result<TopicDto>>
{
    public Guid Id { get; set; }
}
