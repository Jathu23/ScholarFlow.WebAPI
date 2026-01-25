using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Teachers.Queries.GetProfile;

/// <summary>
/// Query to get teacher profile
/// </summary>
public class GetTeacherProfileQuery : IRequest<Result<TeacherProfileDto>>
{
    public Guid UserId { get; set; }
}
