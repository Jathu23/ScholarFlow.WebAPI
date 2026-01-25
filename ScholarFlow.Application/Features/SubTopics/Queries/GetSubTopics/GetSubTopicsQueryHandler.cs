using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopics;

public class GetSubTopicsQueryHandler : IRequestHandler<GetSubTopicsQuery, Result<List<SubTopicDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSubTopicsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SubTopicDto>>> Handle(GetSubTopicsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SubTopics
            .Include(st => st.Topic)
            .AsQueryable();

        if (request.TopicId.HasValue)
        {
            query = query.Where(st => st.TopicId == request.TopicId.Value);
        }

        var subTopics = await query
            .OrderBy(st => st.SubTopicName)
            .ToListAsync(cancellationToken);

        var dtos = subTopics.Select(st => new SubTopicDto
        {
            Id = st.Id,
            SubTopicName = st.SubTopicName,
            TopicId = st.TopicId,
            TopicName = st.Topic?.TopicName ?? ""
        }).ToList();

        return Result<List<SubTopicDto>>.Success(dtos);
    }
}
