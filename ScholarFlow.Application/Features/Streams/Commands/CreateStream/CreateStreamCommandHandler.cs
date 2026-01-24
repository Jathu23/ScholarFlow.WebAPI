using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Streams.Commands.CreateStream;

/// <summary>
/// Handler for CreateStreamCommand
/// </summary>
public class CreateStreamCommandHandler : IRequestHandler<CreateStreamCommand, Result<StreamDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateStreamCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StreamDto>> Handle(CreateStreamCommand request, CancellationToken cancellationToken)
    {
        // Check if stream with same name already exists
        var existingStream = _context.Streams
            .FirstOrDefault(s => s.Name.ToLower() == request.Name.ToLower());

        if (existingStream != null)
        {
            return Result<StreamDto>.Failure($"Stream with name '{request.Name}' already exists");
        }

        // Create new stream
        var academicStream = new AcademicStream
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _context.Streams.Add(academicStream);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var streamDto = new StreamDto
        {
            Id = academicStream.Id,
            Name = academicStream.Name,
            CreatedAt = academicStream.CreatedAt
        };

        return Result<StreamDto>.Success(streamDto);
    }
}
