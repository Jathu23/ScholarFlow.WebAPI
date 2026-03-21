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
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? Explanation { get; set; }
    public int Difficulty { get; set; }
    public decimal Marks { get; set; }
    public int OrderIndex { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}
