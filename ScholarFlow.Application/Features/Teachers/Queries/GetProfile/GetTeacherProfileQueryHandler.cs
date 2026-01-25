using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Teachers.Queries.GetProfile;

/// <summary>
/// Handler for GetTeacherProfileQuery
/// </summary>
public class GetTeacherProfileQueryHandler : IRequestHandler<GetTeacherProfileQuery, Result<TeacherProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTeacherProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TeacherProfileDto>> Handle(GetTeacherProfileQuery request, CancellationToken cancellationToken)
    {
        // Get profile
        var profile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result<TeacherProfileDto>.Failure("Teacher profile not found");
        }

        // Map to DTO
        var dto = new TeacherProfileDto
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Qualification = profile.Qualification,
            Bio = profile.Bio
        };

        return Result<TeacherProfileDto>.Success(dto);
    }
}
