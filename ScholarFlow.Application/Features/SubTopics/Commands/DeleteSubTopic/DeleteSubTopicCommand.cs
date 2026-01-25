using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.SubTopics.Commands.DeleteSubTopic;

public class DeleteSubTopicCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
