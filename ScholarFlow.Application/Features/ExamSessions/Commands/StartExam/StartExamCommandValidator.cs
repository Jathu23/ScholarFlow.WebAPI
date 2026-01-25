using FluentValidation;

namespace ScholarFlow.Application.Features.ExamSessions.Commands.StartExam;

public class StartExamCommandValidator : AbstractValidator<StartExamCommand>
{
    public StartExamCommandValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty().WithMessage("Paper is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User is required");
    }
}
