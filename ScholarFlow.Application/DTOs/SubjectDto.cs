namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Subject data transfer object
/// </summary>
public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Streams { get; set; } = new(); // List of stream names
    public List<Guid> StreamIds { get; set; } = new(); // List of stream IDs
}
