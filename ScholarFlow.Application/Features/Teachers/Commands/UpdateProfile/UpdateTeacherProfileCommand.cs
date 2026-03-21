using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Teachers.Commands.UpdateProfile;

/// <summary>
/// Command to update teacher profile
/// </summary>
public class UpdateTeacherProfileCommand : IRequest<Result<TeacherProfileDto>>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid? SubjectId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Qualification { get; set; }
    public string? Bio { get; set; }
}
