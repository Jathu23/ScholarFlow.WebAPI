using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetSubjectById;

/// <summary>
/// Handler for GetSubjectByIdQuery
/// </summary>
public class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubjectDto>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        // Get subject with streams
        var subject = await _context.Subjects
            .Include(s => s.SubjectStreams)
            .ThenInclude(ss => ss.Stream)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (subject == null)
        {
            return Result<SubjectDto>.Failure("Subject not found");
        }

        // Map to DTO
        var dto = new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            StreamIds = subject.SubjectStreams.Select(ss => ss.StreamId).ToList(),
            Streams = subject.SubjectStreams.Select(ss => ss.Stream.Name).ToList()
        };

        return Result<SubjectDto>.Success(dto);
    }
}
