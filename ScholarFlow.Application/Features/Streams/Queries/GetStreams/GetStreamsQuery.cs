using MediatR;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;

namespace ScholarFlow.Application.Features.Streams.Queries.GetStreams;

/// <summary>
/// Query to get all streams
/// </summary>
public class GetStreamsQuery : IRequest<Result<List<StreamDto>>>
{
}
