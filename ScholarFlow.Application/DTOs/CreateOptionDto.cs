namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Input DTO for creating an option
/// </summary>
public class CreateOptionDto
{
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
