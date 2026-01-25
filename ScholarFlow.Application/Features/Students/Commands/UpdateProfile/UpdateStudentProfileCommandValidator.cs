using FluentValidation;

namespace ScholarFlow.Application.Features.Students.Commands.UpdateProfile;

/// <summary>
/// Validator for UpdateStudentProfileCommand
/// </summary>
public class UpdateStudentProfileCommandValidator : AbstractValidator<UpdateStudentProfileCommand>
{
    public UpdateStudentProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.StreamId)
            .NotEmpty().WithMessage("Stream is required");

        RuleFor(x => x.Batch)
            .NotEmpty().WithMessage("Batch is required")
            .MaximumLength(10).WithMessage("Batch must not exceed 10 characters");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("District is required")
            .MaximumLength(50).WithMessage("District must not exceed 50 characters");

        RuleFor(x => x.Medium)
            .NotEmpty().WithMessage("Medium is required")
            .Must(m => new[] { "Tamil", "English", "Sinhala" }.Contains(m))
            .WithMessage("Medium must be Tamil, English, or Sinhala");
    }
}
