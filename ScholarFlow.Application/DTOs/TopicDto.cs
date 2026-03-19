namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Topic data transfer object
/// </summary>
public class TopicDto
{
    public Guid Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}
