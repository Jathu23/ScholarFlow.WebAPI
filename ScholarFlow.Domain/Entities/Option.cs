using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents an answer option for a question
/// </summary>
public class Option : BaseEntity
{
    private string _optionText = string.Empty;

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
    /// Indicates if this is the correct answer
    /// </summary>
    public bool IsCorrect { get; set; }
    
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
