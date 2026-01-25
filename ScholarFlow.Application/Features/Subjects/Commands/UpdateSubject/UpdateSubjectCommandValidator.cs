using FluentValidation;

namespace ScholarFlow.Application.Features.Subjects.Commands.UpdateSubject;

/// <summary>
/// Validator for UpdateSubjectCommand
/// </summary>
public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subject ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subject name is required")
            .MinimumLength(2).WithMessage("Subject name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Subject name must not exceed 100 characters");

        RuleFor(x => x.StreamIds)
            .NotEmpty().WithMessage("At least one stream must be selected")
            .Must(ids => ids != null && ids.Count > 0).WithMessage("At least one stream must be selected");
    }
}
