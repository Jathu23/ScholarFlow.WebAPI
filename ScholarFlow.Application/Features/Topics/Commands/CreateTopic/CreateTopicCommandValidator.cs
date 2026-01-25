using FluentValidation;

namespace ScholarFlow.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Validator for CreateTopicCommand
/// </summary>
public class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.TopicName)
            .NotEmpty().WithMessage("Topic name is required")
            .MinimumLength(2).WithMessage("Topic name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Topic name must not exceed 200 characters");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject is required");
    }
}
