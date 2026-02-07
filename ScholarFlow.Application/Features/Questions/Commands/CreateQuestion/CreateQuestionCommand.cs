using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Questions.Commands.CreateQuestion;

/// <summary>
/// Command to create a question with options
/// </summary>
public class CreateQuestionCommand : IRequest<Result<QuestionDto>>
{
    public Guid PaperId { get; set; }
    public Guid SubTopicId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public int Difficulty { get; set; }
    public decimal Marks { get; set; } = 1;
    public int OrderIndex { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new();
}
