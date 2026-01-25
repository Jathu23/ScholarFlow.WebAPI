using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Students.Commands.CreateProfile;

/// <summary>
/// Handler for CreateStudentProfileCommand
/// </summary>
public class CreateStudentProfileCommandHandler : IRequestHandler<CreateStudentProfileCommand, Result<StudentProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateStudentProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentProfileDto>> Handle(CreateStudentProfileCommand request, CancellationToken cancellationToken)
    {
        // Check if profile already exists for this user
        var existingProfile = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

        if (existingProfile != null)
        {
            return Result<StudentProfileDto>.Failure("Student profile already exists");
        }

        // Check if stream exists
        var stream = await _context.Streams
            .FirstOrDefaultAsync(s => s.Id == request.StreamId, cancellationToken);

        if (stream == null)
        {
            return Result<StudentProfileDto>.Failure("Stream not found");
        }

        // Create student profile
        var profile = new StudentProfile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FullName = request.FullName,
            StreamId = request.StreamId,
            Batch = request.Batch,
            District = request.District,
            Medium = request.Medium
        };

        _context.StudentProfiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new StudentProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            StreamId = profile.StreamId,
            StreamName = stream.Name,
            Batch = profile.Batch,
            District = profile.District,
            Medium = profile.Medium
        };

        return Result<StudentProfileDto>.Success(dto);
    }
}
