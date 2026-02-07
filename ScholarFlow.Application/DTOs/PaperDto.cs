using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Paper data transfer object
/// </summary>
public class PaperDto
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public Guid CreatedByTeacher { get; set; }
    public string CreatedByTeacherName { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
}
