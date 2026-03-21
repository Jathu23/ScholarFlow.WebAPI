using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Application.DTOs;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Questions.Commands.UpdateQuestion;

/// <summary>
/// Handler for UpdateQuestionCommand.
/// </summary>
public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.SubTopic)
            .Include(q => q.Paper)
            .Include(q => q.Options)
            .Include(q => q.Explanations)
                .ThenInclude(e => e.Sections)
            .FirstOrDefaultAsync(q => q.Id == request.Id && !q.IsDeleted, cancellationToken);

        if (question == null)
        {
            return Result<QuestionDto>.Failure("Question not found");
        }

        question.QuestionText = request.QuestionText;
        question.QuestionImageUrl = request.QuestionImageUrl;
        question.Difficulty = request.Difficulty;
        question.Marks = request.Marks;
        question.OrderIndex = request.OrderIndex;

        var existingOptions = question.Options.OrderBy(o => o.OrderIndex).ToList();
        var referencedOptionIds = await _context.UserResponses
            .Where(ur => ur.QuestionId == question.Id && ur.SelectedOptionId.HasValue)
            .Select(ur => ur.SelectedOptionId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var usedOptionIds = new HashSet<Guid>();
        var updatedOptions = new List<Option>(request.Options.Count);

        for (var index = 0; index < request.Options.Count; index++)
        {
            var incoming = request.Options[index];
            Option? target = null;

            if (incoming.Id.HasValue)
            {
                target = existingOptions.FirstOrDefault(o => o.Id == incoming.Id.Value);
            }

            target ??= existingOptions.FirstOrDefault(o => o.OrderIndex == index && !usedOptionIds.Contains(o.Id));

            if (target == null)
            {
                target = new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id
                };

                await _context.Options.AddAsync(target, cancellationToken);
            }

            target.OptionText = incoming.OptionText;
            target.ContentType = incoming.ContentType;
            target.ImageUrl = incoming.ImageUrl;
            target.Equation = incoming.Equation;
            target.IsCorrect = incoming.IsCorrect;
            target.OrderIndex = index;

            usedOptionIds.Add(target.Id);
            updatedOptions.Add(target);
        }

        var optionsToRemove = existingOptions.Where(o => !usedOptionIds.Contains(o.Id)).ToList();
        var blockedDeletes = optionsToRemove.Where(o => referencedOptionIds.Contains(o.Id)).ToList();
        if (blockedDeletes.Count > 0)
        {
            return Result<QuestionDto>.Failure("Cannot remove options that are already used in student responses");
        }

        if (optionsToRemove.Count > 0)
        {
            _context.Options.RemoveRange(optionsToRemove);
        }

        var normalizedExplanation = string.IsNullOrWhiteSpace(request.Explanation)
            ? null
            : request.Explanation.Trim();
        var existingExplanation = question.Explanations.FirstOrDefault();

        if (normalizedExplanation == null)
        {
            if (existingExplanation != null)
            {
                _context.Explanations.Remove(existingExplanation);
            }
        }
        else
        {
            var explanationTitle = normalizedExplanation.Length <= 200
                ? normalizedExplanation
                : normalizedExplanation[..200];

            if (existingExplanation == null)
            {
                existingExplanation = new Explanation
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    AuthorId = question.Paper.CreatedByTeacher,
                    Title = explanationTitle
                };

                _context.Explanations.Add(existingExplanation);
                _context.ExplanationSections.Add(new ExplanationSection
                {
                    Id = Guid.NewGuid(),
                    ExplanationId = existingExplanation.Id,
                    Type = ContentType.Text,
                    Content = normalizedExplanation,
                    OrderIndex = 0
                });
            }
            else
            {
                existingExplanation.Title = explanationTitle;
                var textSection = existingExplanation.Sections
                    .Where(s => s.Type == ContentType.Text || s.Type == ContentType.Formula || s.Type == ContentType.Code)
                    .OrderBy(s => s.OrderIndex)
                    .FirstOrDefault();

                if (textSection == null)
                {
                    _context.ExplanationSections.Add(new ExplanationSection
                    {
                        Id = Guid.NewGuid(),
                        ExplanationId = existingExplanation.Id,
                        Type = ContentType.Text,
                        Content = normalizedExplanation,
                        OrderIndex = 0
                    });
                }
                else
                {
                    textSection.Content = normalizedExplanation;
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new QuestionDto
        {
            Id = question.Id,
            PaperId = question.PaperId,
            SubTopicId = question.SubTopicId,
            SubTopicName = question.SubTopic?.SubTopicName ?? string.Empty,
            QuestionText = question.QuestionText,
            QuestionImageUrl = question.QuestionImageUrl,
            Explanation = normalizedExplanation,
            Difficulty = question.Difficulty,
            Marks = question.Marks,
            OrderIndex = question.OrderIndex,
            Options = updatedOptions.Select(o => new OptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                ContentType = o.ContentType,
                ImageUrl = o.ImageUrl,
                Equation = o.Equation,
                IsCorrect = o.IsCorrect,
                OrderIndex = o.OrderIndex
            }).OrderBy(o => o.OrderIndex).ToList()
        };

        return Result<QuestionDto>.Success(dto);
    }
}
