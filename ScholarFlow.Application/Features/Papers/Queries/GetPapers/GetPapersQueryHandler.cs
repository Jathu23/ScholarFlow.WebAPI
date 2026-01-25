using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Papers.Queries.GetPapers;

public class GetPapersQueryHandler : IRequestHandler<GetPapersQuery, Result<List<PaperDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPapersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PaperDto>>> Handle(GetPapersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Papers
            .Include(p => p.Subject)
            .Include(p => p.Creator)
            .Include(p => p.Questions)
            .AsQueryable();

        // Apply filters
        if (request.SubjectId.HasValue)
        {
            query = query.Where(p => p.SubjectId == request.SubjectId.Value);
        }

        if (request.Year.HasValue)
        {
            query = query.Where(p => p.Year == request.Year.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(p => p.Type == request.Type.Value);
        }

        var papers = await query
            .OrderByDescending(p => p.Year)
            .ThenBy(p => p.Subject.Name)
            .ToListAsync(cancellationToken);

        var dtos = papers.Select(p => new PaperDto
        {
            Id = p.Id,
            SubjectId = p.SubjectId,
            SubjectName = p.Subject?.Name ?? "",
            Year = p.Year,
            Type = p.Type,
            TypeName = p.Type.ToString(),
            CreatedByTeacher = p.CreatedByTeacher,
            CreatedByTeacherName = p.Creator?.UserName ?? "",
            QuestionCount = p.Questions.Count
        }).ToList();

        return Result<List<PaperDto>>.Success(dtos);
    }
}
