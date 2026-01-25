using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.SubTopics.Commands.CreateSubTopic;

public class CreateSubTopicCommand : IRequest<Result<SubTopicDto>>
{
    public string SubTopicName { get; set; } = string.Empty;
    public Guid TopicId { get; set; }
}
