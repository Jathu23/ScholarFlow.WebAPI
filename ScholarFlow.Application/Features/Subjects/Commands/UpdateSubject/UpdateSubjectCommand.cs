using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Commands.UpdateSubject;

/// <summary>
/// Command to update a subject
/// </summary>
public class UpdateSubjectCommand : IRequest<Result<SubjectDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Guid> StreamIds { get; set; } = new(); // Updated stream associations
}
