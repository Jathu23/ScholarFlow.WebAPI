using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Streams.Queries.GetStreams;

/// <summary>
/// Handler for GetStreamsQuery
/// </summary>
public class GetStreamsQueryHandler : IRequestHandler<GetStreamsQuery, Result<List<StreamDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetStreamsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<StreamDto>>> Handle(GetStreamsQuery request, CancellationToken cancellationToken)
    {
        var streams = await _context.Streams
            .OrderBy(s => s.Name)
            .Select(s => new StreamDto
            {
                Id = s.Id,
                Name = s.Name,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<StreamDto>>.Success(streams);
    }
}
