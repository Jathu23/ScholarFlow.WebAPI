using FluentValidation;

namespace ScholarFlow.Application.Features.Subjects.Commands.CreateSubject;

/// <summary>
/// Validator for CreateSubjectCommand
/// </summary>
public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subject name is required")
            .MinimumLength(2).WithMessage("Subject name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Subject name must not exceed 100 characters");

        RuleFor(x => x.StreamIds)
            .NotEmpty().WithMessage("At least one stream must be selected")
            .Must(ids => ids != null && ids.Count > 0).WithMessage("At least one stream must be selected");
    }
}
