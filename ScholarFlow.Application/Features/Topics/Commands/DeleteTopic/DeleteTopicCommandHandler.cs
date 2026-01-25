using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Topics.Commands.DeleteTopic;

/// <summary>
/// Handler for DeleteTopicCommand
/// </summary>
public class DeleteTopicCommandHandler : IRequestHandler<DeleteTopicCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTopicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteTopicCommand request, CancellationToken cancellationToken)
    {
        // Find topic
        var topic = await _context.Topics
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (topic == null)
        {
            return Result<bool>.Failure("Topic not found");
        }

        // Soft delete
        topic.IsDeleted = true;
        topic.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
