using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Commands.DeleteSubject;

/// <summary>
/// Handler for DeleteSubjectCommand
/// </summary>
public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        // Find subject
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (subject == null)
        {
            return Result<bool>.Failure("Subject not found");
        }

        // Soft delete (set IsDeleted = true)
        subject.IsDeleted = true;
        subject.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
