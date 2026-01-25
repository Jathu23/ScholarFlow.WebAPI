using FluentValidation;

namespace ScholarFlow.Application.Features.SubTopics.Commands.UpdateSubTopic;

public class UpdateSubTopicCommandValidator : AbstractValidator<UpdateSubTopicCommand>
{
    public UpdateSubTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("SubTopic ID is required");

        RuleFor(x => x.SubTopicName)
            .NotEmpty().WithMessage("SubTopic name is required")
            .MinimumLength(2).WithMessage("SubTopic name must be at least 2 characters")
            .MaximumLength(200).WithMessage("SubTopic name must not exceed 200 characters");

        RuleFor(x => x.TopicId)
            .NotEmpty().WithMessage("Topic is required");
    }
}
