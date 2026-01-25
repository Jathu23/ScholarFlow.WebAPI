using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Questions.Queries.GetQuestionsByPaper;

/// <summary>
/// Query to get all questions for a paper
/// </summary>
public class GetQuestionsByPaperQuery : IRequest<Result<List<QuestionDto>>>
{
    public Guid PaperId { get; set; }
}
