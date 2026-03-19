using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Papers.Commands.UpdatePaper;

public class UpdatePaperCommandHandler : IRequestHandler<UpdatePaperCommand, Result<PaperDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdatePaperCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaperDto>> Handle(UpdatePaperCommand request, CancellationToken cancellationToken)
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

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return Result<PaperDto>.Failure("Subject not found");
        }

        var duplicatePaper = await _context.Papers
            .FirstOrDefaultAsync(p => p.Id != request.Id &&
                                     p.SubjectId == request.SubjectId &&
                                     p.Year == request.Year &&
                                     p.Type == request.Type,
                                cancellationToken);

        if (duplicatePaper != null)
        {
            return Result<PaperDto>.Failure($"{request.Type} for {subject.Name} ({request.Year}) already exists");
        }

        paper.SubjectId = request.SubjectId;
        paper.Year = request.Year;
        paper.Type = request.Type;
        paper.Title = request.Title;
        paper.TimeLimit = request.TimeLimit;
        paper.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new PaperDto
        {
            Id = paper.Id,
            SubjectId = paper.SubjectId,
            SubjectName = subject.Name,
            Year = paper.Year,
            Type = paper.Type.ToString(),
            Title = paper.Title,
            TimeLimit = paper.TimeLimit,
            CreatedByTeacher = paper.CreatedByTeacher,
            CreatedByTeacherName = paper.Creator?.UserName ?? string.Empty,
            QuestionCount = paper.Questions.Count
        };

        return Result<PaperDto>.Success(dto);
    }
}
