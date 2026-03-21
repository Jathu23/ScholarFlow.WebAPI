using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetSubjects;

/// <summary>
/// Handler for GetSubjectsQuery
/// </summary>
public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, Result<List<SubjectDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SubjectDto>>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        // Query subjects with their streams
        var query = _context.Subjects
            .Include(s => s.SubjectStreams)
            .ThenInclude(ss => ss.Stream)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        // Filter by stream if provided
        if (request.StreamId.HasValue)
        {
            query = query.Where(s => s.SubjectStreams.Any(ss => ss.StreamId == request.StreamId.Value));
        }

        var subjects = await query
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var dtos = subjects.Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            StreamIds = s.SubjectStreams.Select(ss => ss.StreamId).ToList(),
            Streams = s.SubjectStreams.Select(ss => ss.Stream.Name).ToList()
        }).ToList();

        return Result<List<SubjectDto>>.Success(dtos);
    }
}
