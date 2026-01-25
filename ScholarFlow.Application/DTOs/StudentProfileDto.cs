namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Student profile data transfer object
/// </summary>
public class StudentProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid StreamId { get; set; }
    public string StreamName { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Medium { get; set; } = string.Empty;
}
