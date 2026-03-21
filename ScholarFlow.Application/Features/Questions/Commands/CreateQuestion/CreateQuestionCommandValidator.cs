using FluentValidation;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.Features.Questions.Commands.CreateQuestion;

/// <summary>
/// Validator for CreateQuestionCommand
/// </summary>
public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty().WithMessage("Paper is required");

        RuleFor(x => x.SubTopicId)
            .NotEmpty().WithMessage("SubTopic is required");

        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MinimumLength(5).WithMessage("Question text must be at least 5 characters")
            .MaximumLength(2000).WithMessage("Question text must not exceed 2000 characters");

        RuleFor(x => x.Difficulty)
            .InclusiveBetween(1, 10).WithMessage("Difficulty must be between 1 and 10");

        RuleFor(x => x.Explanation)
            .MaximumLength(2000).WithMessage("Explanation must not exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Explanation));

        // Validate options if provided
        RuleFor(x => x.Options)
            .Must(options => options == null || options.Count == 0 || options.Count >= 2)
            .WithMessage("If options are provided, there must be at least 2 options")
            .When(x => x.Options != null && x.Options.Count > 0);

        RuleFor(x => x.Options)
            .Must(options => options == null || options.Count == 0 || options.Count(o => o.IsCorrect) == 1)
            .WithMessage("Exactly one option must be marked as correct")
            .When(x => x.Options != null && x.Options.Count > 0);

        RuleForEach(x => x.Options)
            .ChildRules(option =>
            {
                option.RuleFor(o => o.ContentType)
                    .IsInEnum().WithMessage("Option content type is invalid");

                option.RuleFor(o => o.OptionText)
                    .MaximumLength(500).WithMessage("Option text must not exceed 500 characters");

                option.RuleFor(o => o.Equation)
                    .MaximumLength(2000).WithMessage("Option equation must not exceed 2000 characters")
                    .When(o => !string.IsNullOrWhiteSpace(o.Equation));

                option.RuleFor(o => o)
                    .Must(o => !string.IsNullOrWhiteSpace(o.OptionText))
                    .WithMessage("Text option must include option text")
                    .When(o => o.ContentType == OptionContentType.Text);

                option.RuleFor(o => o)
                    .Must(o => !string.IsNullOrWhiteSpace(o.ImageUrl))
                    .WithMessage("Image option must include image URL")
                    .When(o => o.ContentType == OptionContentType.Image);

                option.RuleFor(o => o)
                    .Must(o => !string.IsNullOrWhiteSpace(o.Equation))
                    .WithMessage("Equation option must include equation content")
                    .When(o => o.ContentType == OptionContentType.Equation);

                option.RuleFor(o => o)
                    .Must(o => !string.IsNullOrWhiteSpace(o.OptionText) &&
                               (!string.IsNullOrWhiteSpace(o.ImageUrl) || !string.IsNullOrWhiteSpace(o.Equation)))
                    .WithMessage("Mixed option must include text and either image or equation")
                    .When(o => o.ContentType == OptionContentType.Mixed);
            })
            .When(x => x.Options != null && x.Options.Count > 0);
    }
}
