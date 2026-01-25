using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Commands.CreateSubject;

/// <summary>
/// Command to create a new subject
/// </summary>
public class CreateSubjectCommand : IRequest<Result<SubjectDto>>
{
    public string Name { get; set; } = string.Empty;
    public List<Guid> StreamIds { get; set; } = new(); // Streams to associate with this subject
}
