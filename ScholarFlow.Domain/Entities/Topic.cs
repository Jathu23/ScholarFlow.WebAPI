using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a topic within a subject
/// </summary>
public class Topic : AuditableEntity
{
    private string _topicName = string.Empty;

    /// <summary>
    /// Foreign key to Subject
    /// </summary>
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Name of the topic
    /// </summary>
    public string TopicName 
    { 
        get => _topicName;
        set => _topicName = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Display order index within the subject
    /// </summary>
    public int OrderIndex { get; set; }
    
    // Navigation properties
    private readonly List<SubTopic> _subTopics = new();

    /// <summary>
    /// Parent subject
    /// </summary>
    public Subject Subject { get; set; } = null!;

    /// <summary>
    /// Sub-topics under this topic
    /// </summary>
    public IReadOnlyCollection<SubTopic> SubTopics => _subTopics.AsReadOnly();

    internal void AddSubTopic(SubTopic subTopic) => _subTopics.Add(subTopic);
}
