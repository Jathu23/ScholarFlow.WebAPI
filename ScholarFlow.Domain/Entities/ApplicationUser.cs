using Microsoft.AspNetCore.Identity;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Application user entity extending Identity with Guid primary key
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Student profile (only populated for users with Student role)
    /// </summary>
    public StudentProfile? StudentProfile { get; set; }

    /// <summary>
    /// Teacher profile (only populated for users with Teacher role)
    /// </summary>
    public TeacherProfile? TeacherProfile { get; set; }
    
    /// <summary>
    /// Papers created by this teacher
    /// </summary>
    public ICollection<Paper> CreatedPapers { get; set; } = new List<Paper>();

    /// <summary>
    /// Explanations authored by this teacher
    /// </summary>
    public ICollection<Explanation> Explanations { get; set; } = new List<Explanation>();

    /// <summary>
    /// Exam sessions taken by this student
    /// </summary>
    public ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();
}
