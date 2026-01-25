using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.SubTopics.Commands.UpdateSubTopic;

public class UpdateSubTopicCommand : IRequest<Result<SubTopicDto>>
{
    public Guid Id { get; set; }
    public string SubTopicName { get; set; } = string.Empty;
    public Guid TopicId { get; set; }
}
