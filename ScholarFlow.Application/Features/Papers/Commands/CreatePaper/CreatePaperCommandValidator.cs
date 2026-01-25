using FluentValidation;

namespace ScholarFlow.Application.Features.Papers.Commands.CreatePaper;

public class CreatePaperCommandValidator : AbstractValidator<CreatePaperCommand>
{
    public CreatePaperCommandValidator()
    {
        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject is required");

        RuleFor(x => x.Year)
            .GreaterThan(2000).WithMessage("Year must be after 2000")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year cannot be in the future");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid paper type");

        RuleFor(x => x.CreatedByTeacher)
            .NotEmpty().WithMessage("Creator is required");
    }
}
