using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopics;

public class GetSubTopicsQuery : IRequest<Result<List<SubTopicDto>>>
{
    public Guid? TopicId { get; set; }
}
