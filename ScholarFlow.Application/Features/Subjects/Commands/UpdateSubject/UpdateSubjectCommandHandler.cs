using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Commands.UpdateSubject;

/// <summary>
/// Handler for UpdateSubjectCommand
/// </summary>
public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubjectDto>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        // Find subject
        var subject = await _context.Subjects
            .Include(s => s.SubjectStreams)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (subject == null)
        {
            return Result<SubjectDto>.Failure("Subject not found");
        }

        // Check if all streams exist
        var streams = await _context.Streams
            .Where(s => request.StreamIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (streams.Count != request.StreamIds.Count)
        {
            return Result<SubjectDto>.Failure("One or more streams not found");
        }

        // Check for duplicate name (exclude current subject)
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name.ToLower() == request.Name.ToLower() && s.Id != request.Id, 
                                cancellationToken);

        if (existingSubject != null)
        {
            return Result<SubjectDto>.Failure($"Subject '{request.Name}' already exists");
        }

        // Update subject name
        subject.Name = request.Name;

        // Remove existing stream associations
        var existingAssociations = await _context.SubjectStreams
            .Where(ss => ss.SubjectId == request.Id)
            .ToListAsync(cancellationToken);

        _context.SubjectStreams.RemoveRange(existingAssociations);

        // Add new stream associations
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
