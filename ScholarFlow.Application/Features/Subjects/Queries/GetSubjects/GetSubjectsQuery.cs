using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetSubjects;

/// <summary>
/// Query to get all subjects with optional stream filter
/// </summary>
public class GetSubjectsQuery : IRequest<Result<List<SubjectDto>>>
{
    public Guid? StreamId { get; set; }
}
