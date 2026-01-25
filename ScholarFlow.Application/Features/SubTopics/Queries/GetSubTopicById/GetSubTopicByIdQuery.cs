using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopicById;

public class GetSubTopicByIdQuery : IRequest<Result<SubTopicDto>>
{
    public Guid Id { get; set; }
}
