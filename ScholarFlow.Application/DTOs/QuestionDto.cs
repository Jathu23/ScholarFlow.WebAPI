namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Question data transfer object
/// </summary>
public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public Guid SubTopicId { get; set; }
    public string SubTopicName { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public int Difficulty { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}
