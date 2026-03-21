using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Topics.Queries.GetTopicById;

/// <summary>
/// Handler for GetTopicByIdQuery
/// </summary>
public class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTopicByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TopicDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        // Get topic with subject
        var topic = await _context.Topics
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (topic == null)
        {
            return Result<TopicDto>.Failure("Topic not found");
        }

        // Map to DTO
        var dto = new TopicDto
        {
            Id = topic.Id,
            TopicName = topic.TopicName,
            OrderIndex = topic.OrderIndex,
            SubjectId = topic.SubjectId,
            SubjectName = topic.Subject?.Name ?? ""
        };

        return Result<TopicDto>.Success(dto);
    }
}
