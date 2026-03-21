using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Topics.Commands.UpdateTopic;

/// <summary>
/// Command to update a topic
/// </summary>
public class UpdateTopicCommand : IRequest<Result<TopicDto>>
{
    public Guid Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public Guid SubjectId { get; set; }
}
