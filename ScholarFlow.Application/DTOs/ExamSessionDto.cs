using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.DTOs;

/// <summary>
/// ExamSession data transfer object
/// </summary>
public class ExamSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid PaperId { get; set; }
    public string PaperTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal FinalScore { get; set; }
    public ExamSessionStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public TimeSpan? Duration { get; set; }
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public List<ExamSessionResponseDto> Responses { get; set; } = new();
}

public class ExamSessionResponseDto
{
    public Guid QuestionId { get; set; }
    public Guid? SelectedOptionId { get; set; }
}
