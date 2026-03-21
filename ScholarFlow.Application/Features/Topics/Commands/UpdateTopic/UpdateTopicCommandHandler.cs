using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Topics.Commands.UpdateTopic;

/// <summary>
/// Handler for UpdateTopicCommand
/// </summary>
public class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, Result<TopicDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTopicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TopicDto>> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        // Find topic
        var topic = await _context.Topics
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (topic == null)
        {
            return Result<TopicDto>.Failure("Topic not found");
        }

        // Check if subject exists
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return Result<TopicDto>.Failure("Subject not found");
        }

        // Check for duplicate name in the same subject (exclude current topic)
        var existingTopic = await _context.Topics
            .FirstOrDefaultAsync(t => t.SubjectId == request.SubjectId && 
                                     t.TopicName.ToLower() == request.TopicName.ToLower() &&
                                     t.Id != request.Id, 
                                cancellationToken);

        if (existingTopic != null)
        {
            return Result<TopicDto>.Failure($"Topic '{request.TopicName}' already exists in this subject");
        }

        // Update topic
        topic.TopicName = request.TopicName;
        topic.OrderIndex = request.OrderIndex;
        topic.SubjectId = request.SubjectId;

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new TopicDto
        {
            Id = topic.Id,
            TopicName = topic.TopicName,
            OrderIndex = topic.OrderIndex,
            SubjectId = topic.SubjectId,
            SubjectName = subject.Name
        };

        return Result<TopicDto>.Success(dto);
    }
}
