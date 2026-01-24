namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Join entity for many-to-many relationship between Subject and Stream
/// </summary>
public class SubjectStream
{
    /// <summary>
    /// Foreign key to Subject
    /// </summary>
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Foreign key to Stream
    /// </summary>
    public Guid StreamId { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Subject in this relationship
    /// </summary>
    public Subject Subject { get; set; } = null!;

    /// <summary>
    /// Stream in this relationship
    /// </summary>
    public AcademicStream Stream { get; set; } = null!;
}
