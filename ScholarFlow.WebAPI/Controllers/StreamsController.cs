using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Features.Streams.Commands.CreateStream;
using ScholarFlow.Application.Features.Streams.Queries.GetStreams;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Streams API Controller using CQRS pattern
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StreamsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public StreamsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    /// <summary>
    /// Get all streams
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetStreams(CancellationToken cancellationToken)
    {
        var query = new GetStreamsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new stream
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateStream([FromBody] CreateStreamCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetStreams), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Update stream name
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStream(Guid id, [FromBody] CreateStreamCommand command, CancellationToken cancellationToken)
    {
        var stream = await _context.Streams.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (stream == null)
        {
            return NotFound(new { error = "Stream not found" });
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest(new { error = "Stream name is required" });
        }

        stream.Name = command.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            id = stream.Id,
            name = stream.Name,
            createdAt = stream.CreatedAt
        });
    }

    /// <summary>
    /// Soft delete stream
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStream(Guid id, CancellationToken cancellationToken)
    {
        var stream = await _context.Streams.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (stream == null)
        {
            return NotFound(new { error = "Stream not found" });
        }

        _context.Streams.Remove(stream);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "Stream deleted successfully" });
    }

    /// <summary>
    /// Link subject to stream
    /// </summary>
    [HttpPost("{streamId}/subjects/{subjectId}")]
    public async Task<IActionResult> AddSubjectToStream(Guid streamId, Guid subjectId, CancellationToken cancellationToken)
    {
        var streamExists = await _context.Streams.AnyAsync(s => s.Id == streamId, cancellationToken);
        var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId, cancellationToken);

        if (!streamExists || !subjectExists)
        {
            return NotFound(new { error = "Stream or subject not found" });
        }

        var alreadyLinked = await _context.SubjectStreams
            .AnyAsync(ss => ss.StreamId == streamId && ss.SubjectId == subjectId, cancellationToken);

        if (alreadyLinked)
        {
            return BadRequest(new { error = "Subject is already linked to this stream" });
        }

        _context.SubjectStreams.Add(new SubjectStream
        {
            StreamId = streamId,
            SubjectId = subjectId
        });

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { message = "Subject linked to stream successfully" });
    }

    /// <summary>
    /// Unlink subject from stream
    /// </summary>
    [HttpDelete("{streamId}/subjects/{subjectId}")]
    public async Task<IActionResult> RemoveSubjectFromStream(Guid streamId, Guid subjectId, CancellationToken cancellationToken)
    {
        var relation = await _context.SubjectStreams
            .FirstOrDefaultAsync(ss => ss.StreamId == streamId && ss.SubjectId == subjectId, cancellationToken);

        if (relation == null)
        {
            return NotFound(new { error = "Stream-subject relationship not found" });
        }

        _context.SubjectStreams.Remove(relation);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "Subject removed from stream successfully" });
    }
}
