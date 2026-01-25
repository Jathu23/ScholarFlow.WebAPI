using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.SubTopics.Commands.UpdateSubTopic;

public class UpdateSubTopicCommandHandler : IRequestHandler<UpdateSubTopicCommand, Result<SubTopicDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubTopicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubTopicDto>> Handle(UpdateSubTopicCommand request, CancellationToken cancellationToken)
    {
        var subTopic = await _context.SubTopics
            .FirstOrDefaultAsync(st => st.Id == request.Id, cancellationToken);

        if (subTopic == null)
        {
            return Result<SubTopicDto>.Failure("SubTopic not found");
        }

        var topic = await _context.Topics
            .FirstOrDefaultAsync(t => t.Id == request.TopicId, cancellationToken);

        if (topic == null)
        {
            return Result<SubTopicDto>.Failure("Topic not found");
        }

        var existingSubTopic = await _context.SubTopics
            .FirstOrDefaultAsync(st => st.TopicId == request.TopicId && 
                                      st.SubTopicName.ToLower() == request.SubTopicName.ToLower() &&
                                      st.Id != request.Id, 
                                cancellationToken);

        if (existingSubTopic != null)
        {
            return Result<SubTopicDto>.Failure($"SubTopic '{request.SubTopicName}' already exists in this topic");
        }

        subTopic.SubTopicName = request.SubTopicName;
        subTopic.TopicId = request.TopicId;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new SubTopicDto
        {
            Id = subTopic.Id,
            SubTopicName = subTopic.SubTopicName,
            TopicId = subTopic.TopicId,
            TopicName = topic.TopicName
        };

        return Result<SubTopicDto>.Success(dto);
    }
}
