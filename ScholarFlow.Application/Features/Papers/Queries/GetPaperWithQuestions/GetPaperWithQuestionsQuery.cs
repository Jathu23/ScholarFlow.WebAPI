using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Papers.Queries.GetPaperWithQuestions;

public record GetPaperWithQuestionsQuery(Guid Id) : IRequest<Result<PaperDetailDto>>;
