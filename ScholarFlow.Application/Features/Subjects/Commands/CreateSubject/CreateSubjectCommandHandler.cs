using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Commands.CreateSubject;

/// <summary>
/// Handler for CreateSubjectCommand
/// </summary>
public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubjectDto>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        // Check if all streams exist
        var streams = await _context.Streams
            .Where(s => request.StreamIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (streams.Count != request.StreamIds.Count)
        {
            return Result<SubjectDto>.Failure("One or more streams not found");
        }

        // Check for duplicate subject name
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingSubject != null)
        {
            return Result<SubjectDto>.Failure($"Subject '{request.Name}' already exists");
        }

        // Create subject
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _context.Subjects.Add(subject);

        // Create SubjectStream relationships
        foreach (var streamId in request.StreamIds)
        {
            var subjectStream = new SubjectStream
            {
                SubjectId = subject.Id,
                StreamId = streamId
            };
            _context.SubjectStreams.Add(subjectStream);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            StreamIds = request.StreamIds,
            Streams = streams.Select(s => s.Name).ToList()
        };

        return Result<SubjectDto>.Success(dto);
    }
}
