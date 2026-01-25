using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Students.Queries.GetProfile;

/// <summary>
/// Query to get student profile
/// </summary>
public class GetStudentProfileQuery : IRequest<Result<StudentProfileDto>>
{
    public Guid UserId { get; set; }
}
