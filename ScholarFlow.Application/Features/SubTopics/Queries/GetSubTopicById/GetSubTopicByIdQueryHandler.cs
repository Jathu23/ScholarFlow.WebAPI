using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopicById;

public class GetSubTopicByIdQueryHandler : IRequestHandler<GetSubTopicByIdQuery, Result<SubTopicDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubTopicByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubTopicDto>> Handle(GetSubTopicByIdQuery request, CancellationToken cancellationToken)
    {
        var subTopic = await _context.SubTopics
            .Include(st => st.Topic)
            .FirstOrDefaultAsync(st => st.Id == request.Id, cancellationToken);

        if (subTopic == null)
        {
            return Result<SubTopicDto>.Failure("SubTopic not found");
        }

        var dto = new SubTopicDto
        {
            Id = subTopic.Id,
            SubTopicName = subTopic.SubTopicName,
            TopicId = subTopic.TopicId,
            TopicName = subTopic.Topic?.TopicName ?? ""
        };

        return Result<SubTopicDto>.Success(dto);
    }
}
