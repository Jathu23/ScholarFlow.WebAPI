using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.Features.Questions.Commands.UpdateQuestion;

/// <summary>
/// Command to update a question and its options.
/// </summary>
public class UpdateQuestionCommand : IRequest<Result<QuestionDto>>
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? Explanation { get; set; }
    public int Difficulty { get; set; }
    public decimal Marks { get; set; }
    public int OrderIndex { get; set; }
    public List<UpdateOptionDto> Options { get; set; } = new();
}

public class UpdateOptionDto
{
    public Guid? Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public OptionContentType ContentType { get; set; } = OptionContentType.Text;
    public string? ImageUrl { get; set; }
    public string? Equation { get; set; }
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}
