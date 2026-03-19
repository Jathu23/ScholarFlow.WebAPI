using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;
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

        // Check if subtopic exists
        var subTopic = await _context.SubTopics
            .FirstOrDefaultAsync(st => st.Id == request.SubTopicId, cancellationToken);

        if (subTopic == null)
        {
            return Result<QuestionDto>.Failure("SubTopic not found");
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
                    ContentType = optionDto.ContentType,
                    ImageUrl = optionDto.ImageUrl,
                    Equation = optionDto.Equation,
                    IsCorrect = optionDto.IsCorrect,
                    OrderIndex = optionDto.OrderIndex
                };

                _context.Options.Add(option);

                optionDtos.Add(new OptionDto
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    ContentType = option.ContentType,
                    ImageUrl = option.ImageUrl,
                    Equation = option.Equation,
                    IsCorrect = option.IsCorrect,
                    OrderIndex = option.OrderIndex
                });
            }
        }

        string? normalizedExplanation = null;
        if (!string.IsNullOrWhiteSpace(request.Explanation))
        {
            normalizedExplanation = request.Explanation.Trim();
            var explanationTitle = normalizedExplanation.Length <= 200
                ? normalizedExplanation
                : normalizedExplanation[..200];

            var explanation = new Explanation
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                AuthorId = paper.CreatedByTeacher,
                Title = explanationTitle
            };

            _context.Explanations.Add(explanation);
            _context.ExplanationSections.Add(new ExplanationSection
            {
                Id = Guid.NewGuid(),
                ExplanationId = explanation.Id,
                Type = ContentType.Text,
                Content = normalizedExplanation,
                OrderIndex = 0
            });
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
            Explanation = normalizedExplanation,
            Difficulty = question.Difficulty,
            Marks = question.Marks,
            OrderIndex = question.OrderIndex,
            Options = optionDtos
        };

        return Result<QuestionDto>.Success(dto);
    }
}
