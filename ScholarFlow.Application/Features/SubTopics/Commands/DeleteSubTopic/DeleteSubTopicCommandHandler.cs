using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.SubTopics.Commands.DeleteSubTopic;

public class DeleteSubTopicCommandHandler : IRequestHandler<DeleteSubTopicCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubTopicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteSubTopicCommand request, CancellationToken cancellationToken)
    {
        var subTopic = await _context.SubTopics
            .FirstOrDefaultAsync(st => st.Id == request.Id, cancellationToken);

        if (subTopic == null)
        {
            return Result<bool>.Failure("SubTopic not found");
        }

        subTopic.IsDeleted = true;
        subTopic.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
