using FluentValidation;

namespace ScholarFlow.Application.Features.SubTopics.Commands.CreateSubTopic;

public class CreateSubTopicCommandValidator : AbstractValidator<CreateSubTopicCommand>
{
    public CreateSubTopicCommandValidator()
    {
        RuleFor(x => x.SubTopicName)
            .NotEmpty().WithMessage("SubTopic name is required")
            .MinimumLength(2).WithMessage("SubTopic name must be at least 2 characters")
            .MaximumLength(200).WithMessage("SubTopic name must not exceed 200 characters");

        RuleFor(x => x.TopicId)
            .NotEmpty().WithMessage("Topic is required");
    }
}
