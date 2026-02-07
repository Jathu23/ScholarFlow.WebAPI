using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a section of content within an explanation
/// </summary>
public class ExplanationSection : BaseEntity
{
    private string _content = string.Empty;

    /// <summary>
    /// Foreign key to Explanation
    /// </summary>
    public Guid ExplanationId { get; set; }

    /// <summary>
    /// Type of content (Text, Video, Image, Audio)
    /// </summary>
    public ContentType Type { get; set; }

    /// <summary>
    /// Content (text, URL, or HTML)
    /// </summary>
    public string Content 
    { 
        get => _content;
        set => _content = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Optional caption or description
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Order of this section in the explanation
    /// </summary>
    public int OrderIndex { get; set; }

    // Navigation property
    /// <summary>
    /// Explanation this section belongs to
    /// </summary>
    public Explanation Explanation { get; set; } = null!;
}
