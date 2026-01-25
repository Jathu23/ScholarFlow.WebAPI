using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.SubTopics.Commands.CreateSubTopic;
using ScholarFlow.Application.Features.SubTopics.Commands.DeleteSubTopic;
using ScholarFlow.Application.Features.SubTopics.Commands.UpdateSubTopic;
using ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopicById;
using ScholarFlow.Application.Features.SubTopics.Queries.GetSubTopics;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// SubTopics API Controller
/// </summary>
[ApiController]
[Route("api/subtopics")]
public class SubTopicsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubTopicsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all subtopics (optionally filter by topic)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] Guid? topicId, CancellationToken cancellationToken)
    {
        var query = new GetSubTopicsQuery { TopicId = topicId };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Get subtopic by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSubTopicByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : NotFound(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Create a new subtopic
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSubTopicCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Update a subtopic
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubTopicCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : BadRequest(new { error = result.ErrorMessage });
    }

    /// <summary>
    /// Delete a subtopic (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSubTopicCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(new { message = "SubTopic deleted successfully" }) 
            : NotFound(new { error = result.ErrorMessage });
    }
}
