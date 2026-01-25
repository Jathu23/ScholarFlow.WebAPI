using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Topics.Commands.DeleteTopic;

/// <summary>
/// Command to delete a topic (soft delete)
/// </summary>
public class DeleteTopicCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
