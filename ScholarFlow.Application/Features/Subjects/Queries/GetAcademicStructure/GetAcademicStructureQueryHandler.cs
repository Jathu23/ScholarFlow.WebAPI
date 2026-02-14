using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetAcademicStructure;

public class GetAcademicStructureQueryHandler : IRequestHandler<GetAcademicStructureQuery, Result<List<AcademicStreamDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAcademicStructureQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AcademicStreamDto>>> Handle(GetAcademicStructureQuery request, CancellationToken cancellationToken)
    {
        var streams = await _context.Streams
            .Include(s => s.SubjectStreams)
                .ThenInclude(ss => ss.Subject)
                    .ThenInclude(sub => sub.Topics)
                        .ThenInclude(t => t.SubTopics)
            .ToListAsync(cancellationToken);

        var dtos = streams.Select(s => new AcademicStreamDto
        {
            StreamId = s.Id,
            StreamName = s.Name,
            Subjects = s.SubjectStreams.Select(ss => new AtomicSubjectDto
            {
                SubjectId = ss.Subject.Id,
                SubjectName = ss.Subject.Name,
                Topics = ss.Subject.Topics.Select(t => new AtomicTopicDto
                {
                    TopicId = t.Id,
                    TopicName = t.TopicName,
                    SubTopics = t.SubTopics.Select(st => new AtomicSubTopicDto
                    {
                        Id = st.Id,
                        Name = st.SubTopicName
                    }).ToList()
                }).ToList()
            }).ToList()
        }).ToList();

        return Result<List<AcademicStreamDto>>.Success(dtos);
    }
}
