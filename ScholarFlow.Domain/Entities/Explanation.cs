using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an explanation for a question (text or video)
/// </summary>
public class Explanation : AuditableEntity
{
    private string _content = string.Empty;

    /// <summary>
    /// Foreign key to Question
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Type of explanation (Text or Video)
    /// </summary>
    public ExplanationType Type { get; set; }

    /// <summary>
    /// Explanation content (supports LaTeX/HTML for text, URL for video)
    /// </summary>
    public string Content 
    { 
        get => _content;
        set => _content = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// User ID of the teacher who authored this explanation
    /// </summary>
    public Guid AuthorId { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Question this explanation is for
    /// </summary>
    public Question Question { get; set; } = null!;

    /// <summary>
    /// Teacher who authored this explanation
    /// </summary>
    public ApplicationUser Author { get; set; } = null!;
}
