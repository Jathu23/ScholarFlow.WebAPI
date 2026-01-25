using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Topics.Queries.GetTopics;

/// <summary>
/// Handler for GetTopicsQuery
/// </summary>
public class GetTopicsQueryHandler : IRequestHandler<GetTopicsQuery, Result<List<TopicDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTopicsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TopicDto>>> Handle(GetTopicsQuery request, CancellationToken cancellationToken)
    {
        // Query topics with subject
        var query = _context.Topics
            .Include(t => t.Subject)
            .AsQueryable();

        // Filter by subject if provided
        if (request.SubjectId.HasValue)
        {
            query = query.Where(t => t.SubjectId == request.SubjectId.Value);
        }

        var topics = await query
            .OrderBy(t => t.TopicName)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var dtos = topics.Select(t => new TopicDto
        {
            Id = t.Id,
            TopicName = t.TopicName,
            SubjectId = t.SubjectId,
            SubjectName = t.Subject?.Name ?? ""
        }).ToList();

        return Result<List<TopicDto>>.Success(dtos);
    }
}
