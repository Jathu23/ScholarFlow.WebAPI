using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an exam paper (past paper or model paper)
/// </summary>
public class Paper : AuditableEntity
{
    /// <summary>
    /// Foreign key to Subject
    /// </summary>
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Year of the paper
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Type of paper (PastPaper or ModelPaper)
    /// </summary>
    public PaperType Type { get; set; }

    /// <summary>
    /// Title of the paper
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Time limit in minutes
    /// </summary>
    public int TimeLimit { get; set; }

    /// <summary>
    /// User ID of the teacher who created this paper
    /// </summary>
    public Guid CreatedByTeacher { get; set; }
    
    // Navigation properties
    private readonly List<Question> _questions = new();
    private readonly List<ExamSession> _examSessions = new();

    /// <summary>
    /// Subject this paper belongs to
    /// </summary>
    public Subject Subject { get; set; } = null!;

    /// <summary>
    /// Teacher who created this paper
    /// </summary>
    public ApplicationUser Creator { get; set; } = null!;

    /// <summary>
    /// Questions in this paper
    /// </summary>
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    /// <summary>
    /// Exam sessions using this paper
    /// </summary>
    public IReadOnlyCollection<ExamSession> ExamSessions => _examSessions.AsReadOnly();

    internal void AddQuestion(Question question) => _questions.Add(question);
    internal void AddExamSession(ExamSession session) => _examSessions.Add(session);
}
