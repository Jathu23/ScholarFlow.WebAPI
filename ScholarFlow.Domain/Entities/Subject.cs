using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an academic subject (e.g., Mathematics, Physics, Chemistry)
/// </summary>
public class Subject : AuditableEntity
{
    private string _name = string.Empty;

    /// <summary>
    /// Name of the subject
    /// </summary>
    public string Name 
    { 
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }
    
    // Navigation properties with backing fields
    private readonly List<SubjectStream> _subjectStreams = new();
    private readonly List<Topic> _topics = new();
    private readonly List<Paper> _papers = new();

    /// <summary>
    /// Streams associated with this subject
    /// </summary>
    public IReadOnlyCollection<SubjectStream> SubjectStreams => _subjectStreams.AsReadOnly();

    /// <summary>
    /// Topics under this subject
    /// </summary>
    public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

    /// <summary>
    /// Papers for this subject
    /// </summary>
    public IReadOnlyCollection<Paper> Papers => _papers.AsReadOnly();

    // Internal methods for EF Core
    internal void AddSubjectStream(SubjectStream subjectStream) => _subjectStreams.Add(subjectStream);
    internal void AddTopic(Topic topic) => _topics.Add(topic);
    internal void AddPaper(Paper paper) => _papers.Add(paper);
}
