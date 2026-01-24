using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Streams.Commands.CreateStream;

/// <summary>
/// Command to create a new stream
/// </summary>
public class CreateStreamCommand : IRequest<Result<StreamDto>>
{
    public string Name { get; set; } = string.Empty;
}
