using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Profile information for users with Teacher role
/// </summary>
public class TeacherProfile : BaseEntity
{
    private string _fullName = string.Empty;
    private string _qualification = string.Empty;
    private string _bio = string.Empty;

    /// <summary>
    /// Foreign key to ApplicationUser
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to Subject (teacher's primary subject)
    /// </summary>
    public Guid? SubjectId { get; set; }

    /// <summary>
    /// Full name of the teacher
    /// </summary>
    public string FullName 
    { 
        get => _fullName;
        set => _fullName = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Academic qualifications
    /// </summary>
    public string Qualification 
    { 
        get => _qualification;
        set => _qualification = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Teacher phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Teacher biography
    /// </summary>
    public string Bio 
    { 
        get => _bio;
        set => _bio = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Registration review status
    /// </summary>
    public TeacherRegistrationStatus Status { get; set; } = TeacherRegistrationStatus.Pending;

    /// <summary>
    /// Generated teacher code once accepted
    /// </summary>
    public string? TeacherCode { get; set; }

    /// <summary>
    /// Reason provided when rejected
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Last review time by admin
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    
    // Navigation property
    /// <summary>
    /// Associated user account
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Subject linked to this teacher profile
    /// </summary>
    public Subject? Subject { get; set; }
}
