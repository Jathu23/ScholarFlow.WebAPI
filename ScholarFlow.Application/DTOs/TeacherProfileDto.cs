namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Teacher profile data transfer object
/// </summary>
public class TeacherProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Qualification { get; set; }
    public string? Bio { get; set; }
}
