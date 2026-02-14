using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.DTOs;

public class CreateQuestionDto
{
    public Guid SubTopicId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public int Difficulty { get; set; }
    public decimal Marks { get; set; } = 1;
    public int OrderIndex { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new();
}
