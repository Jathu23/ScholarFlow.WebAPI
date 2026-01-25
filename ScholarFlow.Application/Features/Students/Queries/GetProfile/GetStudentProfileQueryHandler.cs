using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Students.Queries.GetProfile;

/// <summary>
/// Handler for GetStudentProfileQuery
/// </summary>
public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, Result<StudentProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStudentProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentProfileDto>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        // Get profile with stream
        var profile = await _context.StudentProfiles
            .Include(s => s.Stream)
            .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<StudentProfileDto>.Failure("Student profile not found");
        }

        // Map to DTO
        var dto = new StudentProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            StreamId = profile.StreamId,
            StreamName = profile.Stream?.Name ?? "",
            Batch = profile.Batch,
            District = profile.District,
            Medium = profile.Medium
        };

        return Result<StudentProfileDto>.Success(dto);
    }
}
