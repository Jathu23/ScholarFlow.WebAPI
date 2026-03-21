namespace ScholarFlow.Application.DTOs;

using ScholarFlow.Domain.Enums;

/// <summary>
/// Input DTO for creating an option
/// </summary>
public class CreateOptionDto
{
    public string OptionText { get; set; } = string.Empty;
    public OptionContentType ContentType { get; set; } = OptionContentType.Text;
    public string? ImageUrl { get; set; }
    public string? Equation { get; set; }
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}
