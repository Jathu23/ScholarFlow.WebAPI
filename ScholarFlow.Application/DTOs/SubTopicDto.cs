namespace ScholarFlow.Application.DTOs;

/// <summary>
/// SubTopic data transfer object
/// </summary>
public class SubTopicDto
{
    public Guid Id { get; set; }
    public string SubTopicName { get; set; } = string.Empty;
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
}
