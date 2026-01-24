using ScholarFlow.Domain.Entities.Base;

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
    /// Teacher biography
    /// </summary>
    public string Bio 
    { 
        get => _bio;
        set => _bio = value?.Trim() ?? string.Empty;
    }
    
    // Navigation property
    /// <summary>
    /// Associated user account
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}
