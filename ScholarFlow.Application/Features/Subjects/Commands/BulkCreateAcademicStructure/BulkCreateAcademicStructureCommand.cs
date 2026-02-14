using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Subjects.Commands.BulkCreateAcademicStructure;

public class BulkCreateAcademicStructureCommand : IRequest<Result<bool>>
{
    public List<CreateStreamDto> Streams { get; set; } = new();
}
