using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.Features.Papers.Commands.CreatePaper;

public class CreatePaperCommand : IRequest<Result<PaperDto>>
{
    public Guid SubjectId { get; set; }
    public int Year { get; set; }
    public PaperType Type { get; set; }
    public Guid CreatedByTeacher { get; set; } // Will be set from JWT in controller
}
