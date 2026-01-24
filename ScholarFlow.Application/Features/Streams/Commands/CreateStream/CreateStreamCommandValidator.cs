using FluentValidation;

namespace ScholarFlow.Application.Features.Streams.Commands.CreateStream;

/// <summary>
/// Validator for CreateStreamCommand
/// </summary>
public class CreateStreamCommandValidator : AbstractValidator<CreateStreamCommand>
{
    public CreateStreamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Stream name is required")
            .MaximumLength(100).WithMessage("Stream name must not exceed 100 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("Stream name must contain only letters and spaces");
    }
}
