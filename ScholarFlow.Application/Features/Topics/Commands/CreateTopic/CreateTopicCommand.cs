using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Command to create a new topic
/// </summary>
public class CreateTopicCommand : IRequest<Result<TopicDto>>
{
    public string TopicName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public Guid SubjectId { get; set; }
}
