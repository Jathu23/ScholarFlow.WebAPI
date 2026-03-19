using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Students.Commands.UpdateProfile;

/// <summary>
/// Handler for UpdateStudentProfileCommand
/// </summary>
public class UpdateStudentProfileCommandHandler : IRequestHandler<UpdateStudentProfileCommand, Result<StudentProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateStudentProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentProfileDto>> Handle(UpdateStudentProfileCommand request, CancellationToken cancellationToken)
    {
        // Get profile for this user
        var profile = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<StudentProfileDto>.Failure("Student profile not found");
        }

        // Validate stream exists
        var stream = await _context.Streams
            .FirstOrDefaultAsync(s => s.Id == request.StreamId, cancellationToken);

        if (stream == null)
        {
            return Result<StudentProfileDto>.Failure("Stream not found");
        }

        var distinctSelectedSubjectIds = request.SelectedSubjectIds.Distinct().ToList();

        var validSubjectIds = await _context.SubjectStreams
            .Where(ss => ss.StreamId == request.StreamId && distinctSelectedSubjectIds.Contains(ss.SubjectId))
            .Select(ss => ss.SubjectId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (validSubjectIds.Count != distinctSelectedSubjectIds.Count)
        {
            return Result<StudentProfileDto>.Failure("One or more selected subjects are invalid for this stream");
        }

        // Update profile
        profile.FullName = request.FullName;
        profile.StreamId = request.StreamId;
        profile.Batch = request.Batch;
        profile.District = request.District;
        profile.Medium = request.Medium;

        var existingSelections = await _context.StudentSubjectSelections
            .Where(ss => ss.StudentProfileId == profile.Id)
            .ToListAsync(cancellationToken);

        _context.StudentSubjectSelections.RemoveRange(existingSelections);

        var newSelections = distinctSelectedSubjectIds.Select(subjectId => new StudentSubjectSelection
        {
            Id = Guid.NewGuid(),
            StudentProfileId = profile.Id,
            SubjectId = subjectId
        }).ToList();

        _context.StudentSubjectSelections.AddRange(newSelections);

        await _context.SaveChangesAsync(cancellationToken);

        var selectedSubjects = await _context.Subjects
            .Where(s => distinctSelectedSubjectIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => new EnrolledSubjectDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync(cancellationToken);

        // Map to DTO
        var dto = new StudentProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            StreamId = profile.StreamId,
            StreamName = stream.Name,
            Batch = profile.Batch,
            District = profile.District,
            Medium = profile.Medium,
            EnrolledSubjects = selectedSubjects
        };

        return Result<StudentProfileDto>.Success(dto);
    }
}
