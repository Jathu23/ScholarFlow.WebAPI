using MediatR;
using ScholarFlow.Application.Common.Models;

namespace ScholarFlow.Application.Features.Papers.Commands.DeletePaper;

public class DeletePaperCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
