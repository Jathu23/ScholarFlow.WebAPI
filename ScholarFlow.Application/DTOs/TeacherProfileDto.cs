namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Teacher profile data transfer object
/// </summary>
public class TeacherProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Qualification { get; set; }
    public string? Bio { get; set; }
    public string Status { get; set; } = "Pending";
    public string? TeacherCode { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
