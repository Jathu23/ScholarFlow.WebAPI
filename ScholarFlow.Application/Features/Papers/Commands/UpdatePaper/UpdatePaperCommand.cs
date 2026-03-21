using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.Features.Papers.Commands.UpdatePaper;

public class UpdatePaperCommand : IRequest<Result<PaperDto>>
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public int Year { get; set; }
    public PaperType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public Guid UpdatedByTeacher { get; set; }
}
