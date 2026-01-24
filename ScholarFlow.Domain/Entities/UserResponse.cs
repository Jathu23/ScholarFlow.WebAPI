using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a user's response to a question during an exam session
/// </summary>
public class UserResponse : BaseEntity
{
    /// <summary>
    /// Foreign key to ExamSession
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Foreign key to Question
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Foreign key to selected Option (null if not answered)
    /// </summary>
    public Guid? SelectedOptionId { get; set; }

    /// <summary>
    /// Status of the response (Unvisited, Answered, Skipped, MarkedForReview)
    /// </summary>
    public ResponseStatus ResponseStatus { get; set; }

    /// <summary>
    /// Time spent on this question in seconds
    /// </summary>
    public int TimeSpentSeconds { get; set; }

    /// <summary>
    /// Whether the selected answer is correct
    /// </summary>
    public bool IsCorrect { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Exam session this response belongs to
    /// </summary>
    public ExamSession Session { get; set; } = null!;

    /// <summary>
    /// Question being answered
    /// </summary>
    public Question Question { get; set; } = null!;

    /// <summary>
    /// Selected option (null if not answered)
    /// </summary>
    public Option? SelectedOption { get; set; }
}
