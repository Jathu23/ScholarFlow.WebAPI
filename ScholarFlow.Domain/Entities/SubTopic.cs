using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a sub-topic within a topic
/// </summary>
public class SubTopic : AuditableEntity
{
    private string _subTopicName = string.Empty;

    /// <summary>
    /// Foreign key to Topic
    /// </summary>
    public Guid TopicId { get; set; }

    /// <summary>
    /// Name of the sub-topic
    /// </summary>
    public string SubTopicName 
    { 
        get => _subTopicName;
        set => _subTopicName = value?.Trim() ?? string.Empty;
    }
    
    // Navigation properties
    private readonly List<Question> _questions = new();

    /// <summary>
    /// Parent topic
    /// </summary>
    public Topic Topic { get; set; } = null!;

    /// <summary>
    /// Questions under this sub-topic
    /// </summary>
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    internal void AddQuestion(Question question) => _questions.Add(question);
}
