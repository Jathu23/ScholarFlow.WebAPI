using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Streams.Commands.CreateStream;
using ScholarFlow.Application.Features.Streams.Queries.GetStreams;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Streams API Controller using CQRS pattern
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StreamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StreamsController(IMediator mediator)
    {
        _mediator = mediator;
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
}
