using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an explanation for a question with multiple content sections
/// </summary>
public class Explanation : AuditableEntity
{
    /// <summary>
    /// Foreign key to Question
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// User ID of the teacher who authored this explanation
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Optional title for the explanation
    /// </summary>
    public string? Title { get; set; }
    
    // Navigation properties
    private readonly List<ExplanationSection> _sections = new();

    /// <summary>
    /// Question this explanation is for
    /// </summary>
    public Question Question { get; set; } = null!;

    /// <summary>
    /// Teacher who authored this explanation
    /// </summary>
    public ApplicationUser Author { get; set; } = null!;

    /// <summary>
    /// Content sections (text, video, image, audio)
    /// </summary>
    public IReadOnlyCollection<ExplanationSection> Sections => _sections.AsReadOnly();

    internal void AddSection(ExplanationSection section) => _sections.Add(section);
}

