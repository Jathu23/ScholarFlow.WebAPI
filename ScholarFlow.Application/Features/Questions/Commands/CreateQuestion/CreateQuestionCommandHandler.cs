using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Questions.Commands.CreateQuestion;

/// <summary>
/// Handler for CreateQuestionCommand
/// </summary>
public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateQuestionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        // Check if paper exists
        var paper = await _context.Papers
            .FirstOrDefaultAsync(p => p.Id == request.PaperId, cancellationToken);

        if (paper == null)
        {
            return Result<QuestionDto>.Failure("Paper not found");
        }

        // Check if subtopic exists and belongs to the same subject as the paper
        var subTopic = await _context.SubTopics
            .Include(st => st.Topic)
            .FirstOrDefaultAsync(st => st.Id == request.SubTopicId, cancellationToken);

        if (subTopic == null)
        {
            return Result<QuestionDto>.Failure("SubTopic not found");
        }

        if (subTopic.Topic.SubjectId != paper.SubjectId)
        {
             return Result<QuestionDto>.Failure("The selected SubTopic does not belong to the Subject of this Paper.");
        }

        // Create question
        var question = new Question
        {
            Id = Guid.NewGuid(),
            PaperId = request.PaperId,
            SubTopicId = request.SubTopicId,
            QuestionText = request.QuestionText,
            QuestionImageUrl = request.QuestionImageUrl,
            Difficulty = request.Difficulty,
            Marks = request.Marks,
            OrderIndex = request.OrderIndex
        };

        _context.Questions.Add(question);

        // Create options if provided
        var optionDtos = new List<OptionDto>();
        if (request.Options != null && request.Options.Count > 0)
        {
            foreach (var optionDto in request.Options)
            {
                var option = new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    OptionText = optionDto.OptionText,
                    IsCorrect = optionDto.IsCorrect,
                    OrderIndex = optionDto.OrderIndex
                };

                _context.Options.Add(option);

                optionDtos.Add(new OptionDto
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    IsCorrect = option.IsCorrect,
                    OrderIndex = option.OrderIndex
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var dto = new QuestionDto
        {
            Id = question.Id,
            PaperId = question.PaperId,
            SubTopicId = question.SubTopicId,
            SubTopicName = subTopic.SubTopicName,
            QuestionText = question.QuestionText,
            QuestionImageUrl = question.QuestionImageUrl,
            Difficulty = question.Difficulty,
            Marks = question.Marks,
            OrderIndex = question.OrderIndex,
            Options = optionDtos
        };

        return Result<QuestionDto>.Success(dto);
    }
}
