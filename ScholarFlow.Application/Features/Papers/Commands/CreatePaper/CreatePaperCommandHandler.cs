using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Papers.Commands.CreatePaper;

public class CreatePaperCommandHandler : IRequestHandler<CreatePaperCommand, Result<PaperDto>>
{
    private readonly IApplicationDbContext _context;

    public CreatePaperCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaperDto>> Handle(CreatePaperCommand request, CancellationToken cancellationToken)
    {
        // Check if subject exists
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return Result<PaperDto>.Failure("Subject not found");
        }

        // Check if creator exists
        var creator = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CreatedByTeacher, cancellationToken);

        if (creator == null)
        {
            return Result<PaperDto>.Failure("Creator not found");
        }

        // Check for duplicate (same subject, year, type)
        var existingPaper = await _context.Papers
            .FirstOrDefaultAsync(p => p.SubjectId == request.SubjectId && 
                                     p.Year == request.Year &&
                                     p.Type == request.Type, 
                                cancellationToken);

        if (existingPaper != null)
        {
            return Result<PaperDto>.Failure($"{request.Type} for {subject.Name} ({request.Year}) already exists");
        }

        // Create paper
        var paper = new Paper
        {
            Id = Guid.NewGuid(),
            SubjectId = request.SubjectId,
            Year = request.Year,
            Type = request.Type,
            Title = request.Title,
            TimeLimit = request.TimeLimit,
            CreatedByTeacher = request.CreatedByTeacher
        };

        _context.Papers.Add(paper);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
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
            CreatedByTeacherName = creator.UserName ?? "",
            QuestionCount = 0
        };

        return Result<PaperDto>.Success(dto);
    }
}
