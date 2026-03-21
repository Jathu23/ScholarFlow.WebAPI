using FluentValidation;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.StartExam;

public class StartExamCommandValidator : AbstractValidator<StartExamCommand>
{
    public StartExamCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.PaperId != Guid.Empty || x.TopicId.HasValue)
            .WithMessage("Paper or topic is required");

        RuleFor(x => x.QuestionLimit)
            .GreaterThan(0)
            .When(x => x.TopicId.HasValue)
            .WithMessage("Question limit must be greater than zero");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User is required");
    }
}
