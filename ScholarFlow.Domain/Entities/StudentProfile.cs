using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Profile information for users with Student role
/// </summary>
public class StudentProfile : BaseEntity
{
    private string _fullName = string.Empty;
    private string _batch = string.Empty;
    private string _district = string.Empty;
    private string _medium = string.Empty;

    /// <summary>
    /// Foreign key to ApplicationUser
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Full name of the student
    /// </summary>
    public string FullName 
    { 
        get => _fullName;
        set => _fullName = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Foreign key to Stream
    /// </summary>
    public Guid StreamId { get; set; }

    /// <summary>
    /// Academic batch/year
    /// </summary>
    public string Batch 
    { 
        get => _batch;
        set => _batch = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// District of the student
    /// </summary>
    public string District 
    { 
        get => _district;
        set => _district = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Medium of instruction (e.g., Tamil, English)
    /// </summary>
    public string Medium 
    { 
        get => _medium;
        set => _medium = value?.Trim() ?? string.Empty;
    }
    
    // Navigation properties
    /// <summary>
    /// Associated user account
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Academic stream
    /// </summary>
    public AcademicStream Stream { get; set; } = null!;

    /// <summary>
    /// Subjects selected by the student in the chosen stream
    /// </summary>
    public ICollection<StudentSubjectSelection> SelectedSubjects { get; set; } = new List<StudentSubjectSelection>();
}
