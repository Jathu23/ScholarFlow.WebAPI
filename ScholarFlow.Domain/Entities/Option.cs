using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an answer option for a question
/// </summary>
public class Option : BaseEntity
{
    private string _optionText = string.Empty;
    private string? _imageUrl;
    private string? _equation;

    /// <summary>
    /// Foreign key to Question
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Option text (supports LaTeX/HTML)
    /// </summary>
    public string OptionText 
    { 
        get => _optionText;
        set => _optionText = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Option content style (text, image, equation, or mixed)
    /// </summary>
    public OptionContentType ContentType { get; set; } = OptionContentType.Text;

    /// <summary>
    /// Optional image URL for image or mixed options
    /// </summary>
    public string? ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// Optional equation expression (e.g. LaTeX) for equation or mixed options
    /// </summary>
    public string? Equation
    {
        get => _equation;
        set => _equation = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// Indicates if this is the correct answer
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Order of the option (0=A, 1=B, 2=C, 3=D)
    /// </summary>
    public int OrderIndex { get; set; }
    
    // Navigation properties
    private readonly List<UserResponse> _userResponses = new();

    /// <summary>
    /// Question this option belongs to
    /// </summary>
    public Question Question { get; set; } = null!;

    /// <summary>
    /// User responses that selected this option
    /// </summary>
    public IReadOnlyCollection<UserResponse> UserResponses => _userResponses.AsReadOnly();

    internal void AddUserResponse(UserResponse response) => _userResponses.Add(response);
}
