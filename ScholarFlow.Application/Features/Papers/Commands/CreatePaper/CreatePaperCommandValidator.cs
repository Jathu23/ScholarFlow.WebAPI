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

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0).WithMessage("Time limit must be greater than 0")
            .LessThanOrEqualTo(300).WithMessage("Time limit cannot exceed 300 minutes");

        RuleFor(x => x.CreatedByTeacher)
            .NotEmpty().WithMessage("Creator is required");
    }
}
