using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Questions.Commands.DeleteQuestion;

/// <summary>
/// Command to delete a question
/// </summary>
public class DeleteQuestionCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
