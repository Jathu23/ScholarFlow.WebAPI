using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetSubjectById;

/// <summary>
/// Query to get subject by ID
/// </summary>
public class GetSubjectByIdQuery : IRequest<Result<SubjectDto>>
{
    public Guid Id { get; set; }
}
