using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Queries.GetAcademicStructure;

public record GetAcademicStructureQuery : IRequest<Result<List<AcademicStreamDto>>>;
