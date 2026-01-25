namespace ScholarFlow.Application.DTOs;

/// <summary>
/// Option data transfer object
/// </summary>
public class OptionDto
{
    public Guid Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
