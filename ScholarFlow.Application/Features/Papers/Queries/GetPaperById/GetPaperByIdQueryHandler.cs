using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Papers.Queries.GetPaperById;

public class GetPaperByIdQueryHandler : IRequestHandler<GetPaperByIdQuery, Result<PaperDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPaperByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaperDto>> Handle(GetPaperByIdQuery request, CancellationToken cancellationToken)
    {
        var paper = await _context.Papers
            .Include(p => p.Subject)
            .Include(p => p.Creator)
            .Include(p => p.Questions)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (paper == null)
        {
            return Result<PaperDto>.Failure("Paper not found");
        }

        var dto = new PaperDto
        {
            Id = paper.Id,
            SubjectId = paper.SubjectId,
            SubjectName = paper.Subject?.Name ?? "",
            Year = paper.Year,
            Type = paper.Type,
            TypeName = paper.Type.ToString(),
            CreatedByTeacher = paper.CreatedByTeacher,
            CreatedByTeacherName = paper.Creator?.UserName ?? "",
            QuestionCount = paper.Questions.Count
        };

        return Result<PaperDto>.Success(dto);
    }
}
