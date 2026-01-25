using FluentValidation;

namespace ScholarFlow.Application.Features.Topics.Commands.UpdateTopic;

/// <summary>
/// Validator for UpdateTopicCommand
/// </summary>
public class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Topic ID is required");

        RuleFor(x => x.TopicName)
            .NotEmpty().WithMessage("Topic name is required")
            .MinimumLength(2).WithMessage("Topic name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Topic name must not exceed 200 characters");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject is required");
    }
}
