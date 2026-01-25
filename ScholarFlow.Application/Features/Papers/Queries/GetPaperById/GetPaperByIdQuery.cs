using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Papers.Queries.GetPaperById;

public class GetPaperByIdQuery : IRequest<Result<PaperDto>>
{
    public Guid Id { get; set; }
}
