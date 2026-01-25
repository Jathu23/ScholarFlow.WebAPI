using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Subjects.Commands.DeleteSubject;

/// <summary>
/// Command to delete a subject (soft delete)
/// </summary>
public class DeleteSubjectCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
