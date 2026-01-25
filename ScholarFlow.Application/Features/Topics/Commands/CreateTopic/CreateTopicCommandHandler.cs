using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Handler for CreateTopicCommand
/// </summary>
public class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, Result<TopicDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateTopicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TopicDto>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        // Check if subject exists
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return Result<TopicDto>.Failure("Subject not found");
        }

        // Check for duplicate topic name in the same subject
        var existingTopic = await _context.Topics
            .FirstOrDefaultAsync(t => t.SubjectId == request.SubjectId && 
                                     t.TopicName.ToLower() == request.TopicName.ToLower(), 
                                cancellationToken);

        if (existingTopic != null)
        {
            return Result<TopicDto>.Failure($"Topic '{request.TopicName}' already exists in this subject");
        }

        // Create topic
        var topic = new Topic
        {
            Id = Guid.NewGuid(),
            TopicName = request.TopicName,
            SubjectId = request.SubjectId
        };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new TopicDto
        {
            Id = topic.Id,
            TopicName = topic.TopicName,
            SubjectId = topic.SubjectId,
            SubjectName = subject.Name
        };

        return Result<TopicDto>.Success(dto);
    }
}
