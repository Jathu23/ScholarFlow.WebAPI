using FluentValidation;

namespace ScholarFlow.Application.Features.Teachers.Commands.UpdateProfile;

/// <summary>
/// Validator for UpdateTeacherProfileCommand
/// </summary>
public class UpdateTeacherProfileCommandValidator : AbstractValidator<UpdateTeacherProfileCommand>
{
    public UpdateTeacherProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Qualification)
            .MaximumLength(200).WithMessage("Qualification must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Qualification));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }
}
