using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a student's exam session
/// </summary>
public class ExamSession : BaseEntity
{
    /// <summary>
    /// Foreign key to User (Student)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to Paper
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// When the exam session started
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// When the exam session ended (null if still in progress)
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Final score achieved (percentage)
    /// </summary>
    public decimal FinalScore { get; set; }

    /// <summary>
    /// Current status of the exam session
    /// </summary>
    public ExamSessionStatus Status { get; set; }

    /// <summary>
    /// Computed: Duration of the exam session
    /// </summary>
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    
    // Navigation properties
    private readonly List<UserResponse> _userResponses = new();

    /// <summary>
    /// Student taking the exam
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Paper being attempted
    /// </summary>
    public Paper Paper { get; set; } = null!;

    /// <summary>
    /// Responses submitted during this session
    /// </summary>
    public IReadOnlyCollection<UserResponse> UserResponses => _userResponses.AsReadOnly();

    internal void AddUserResponse(UserResponse response) => _userResponses.Add(response);
}
