using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
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

        // Update profile
        profile.FullName = request.FullName;
        profile.StreamId = request.StreamId;
        profile.Batch = request.Batch;
        profile.District = request.District;
        profile.Medium = request.Medium;

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
