using FluentValidation;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Application.Features.Questions.Commands.UpdateQuestion;

/// <summary>
/// Validator for UpdateQuestionCommand.
/// </summary>
public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Question id is required");

        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MinimumLength(5).WithMessage("Question text must be at least 5 characters")
            .MaximumLength(2000).WithMessage("Question text must not exceed 2000 characters");

        RuleFor(x => x.Explanation)
            .MaximumLength(5000).WithMessage("Explanation must not exceed 5000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Explanation));

        RuleFor(x => x.Difficulty)
            .InclusiveBetween(1, 10).WithMessage("Difficulty must be between 1 and 10");

        RuleFor(x => x.Options)
            .Must(options => options != null && options.Count >= 2)
            .WithMessage("At least 2 options are required");

        RuleFor(x => x.Options)
            .Must(options => options != null && options.Count(o => o.IsCorrect) == 1)
            .WithMessage("Exactly one option must be marked as correct");

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
