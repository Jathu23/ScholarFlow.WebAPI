using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Students.Commands.UpdateProfile;

/// <summary>
/// Command to update student profile
/// </summary>
public class UpdateStudentProfileCommand : IRequest<Result<StudentProfileDto>>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid StreamId { get; set; }
    public string Batch { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Medium { get; set; } = string.Empty;
}
