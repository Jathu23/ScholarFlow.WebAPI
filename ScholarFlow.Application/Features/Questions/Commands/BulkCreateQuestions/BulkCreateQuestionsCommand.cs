using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Questions.Commands.BulkCreateQuestions;

public class BulkCreateQuestionsCommand : IRequest<Result<bool>>
{
    public Guid PaperId { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new();
}
