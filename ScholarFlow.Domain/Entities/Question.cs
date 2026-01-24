using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Entities;

/// <summary>
/// Represents a question in an exam paper
/// </summary>
public class Question : AuditableEntity
{
    private string _questionText = string.Empty;
    private string? _questionImageUrl;

    /// <summary>
    /// Foreign key to Paper
    /// </summary>
    public Guid PaperId { get; set; }

    /// <summary>
    /// Foreign key to SubTopic
    /// </summary>
    public Guid SubTopicId { get; set; }

    /// <summary>
    /// Question text (supports LaTeX/HTML)
    /// </summary>
    public string QuestionText 
    { 
        get => _questionText;
        set => _questionText = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Optional image URL for the question
    /// </summary>
    public string? QuestionImageUrl 
    { 
        get => _questionImageUrl;
        set => _questionImageUrl = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    /// <summary>
    /// Difficulty level (1-10)
    /// </summary>
    public int Difficulty { get; set; }
    
    // Navigation properties
    private readonly List<Option> _options = new();
    private readonly List<Explanation> _explanations = new();
    private readonly List<UserResponse> _userResponses = new();

    /// <summary>
    /// Paper this question belongs to
    /// </summary>
    public Paper Paper { get; set; } = null!;

    /// <summary>
    /// Sub-topic this question is categorized under
    /// </summary>
    public SubTopic SubTopic { get; set; } = null!;

    /// <summary>
    /// Answer options for this question
    /// </summary>
    public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

    /// <summary>
    /// Explanations for this question
    /// </summary>
    public IReadOnlyCollection<Explanation> Explanations => _explanations.AsReadOnly();

    /// <summary>
    /// User responses to this question
    /// </summary>
    public IReadOnlyCollection<UserResponse> UserResponses => _userResponses.AsReadOnly();

    internal void AddOption(Option option) => _options.Add(option);
    internal void AddExplanation(Explanation explanation) => _explanations.Add(explanation);
    internal void AddUserResponse(UserResponse response) => _userResponses.Add(response);
}
