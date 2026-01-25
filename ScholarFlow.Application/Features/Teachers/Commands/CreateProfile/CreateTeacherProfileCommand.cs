using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Teachers.Commands.CreateProfile;

/// <summary>
/// Command to create teacher profile
/// </summary>
public class CreateTeacherProfileCommand : IRequest<Result<TeacherProfileDto>>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Qualification { get; set; }
    public string? Bio { get; set; }
}
