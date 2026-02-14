using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Questions.Commands.BulkCreateQuestions;

public class BulkCreateQuestionsCommandHandler : IRequestHandler<BulkCreateQuestionsCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public BulkCreateQuestionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(BulkCreateQuestionsCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Paper
        var paper = await _context.Papers
            .FirstOrDefaultAsync(p => p.Id == request.PaperId, cancellationToken);

        if (paper == null)
            return Result<bool>.Failure("Paper not found");

        if (!request.Questions.Any())
            return Result<bool>.Success(true); // Nothing to do

        // 2. Efficiently Validate SubTopics
        var subTopicIds = request.Questions.Select(q => q.SubTopicId).Distinct().ToList();
        
        // Fetch all SubTopics with their Topics -> Subjects that match the requested IDs
        var validSubTopics = await _context.SubTopics
            .Include(st => st.Topic)
            .Where(st => subTopicIds.Contains(st.Id) && st.Topic.SubjectId == paper.SubjectId)
            .Select(st => st.Id)
            .ToListAsync(cancellationToken);

        // Check if all requested SubTopics are valid
        var invalidSubTopics = subTopicIds.Except(validSubTopics).ToList();
        if (invalidSubTopics.Any())
        {
            return Result<bool>.Failure($"One or more SubTopics do not belong to the Paper's Subject. Invalid SubTopic IDs: {string.Join(", ", invalidSubTopics)}");
        }

        // 3. Create Entities in Bulk

        foreach (var qDto in request.Questions)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                PaperId = paper.Id,
                SubTopicId = qDto.SubTopicId,
                QuestionText = qDto.QuestionText,
                QuestionImageUrl = qDto.QuestionImageUrl,
                Difficulty = qDto.Difficulty,
                Marks = qDto.Marks,
                OrderIndex = qDto.OrderIndex
            };

            _context.Questions.Add(question);

            foreach (var oDto in qDto.Options)
            {
                var option = new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    OptionText = oDto.OptionText,
                    IsCorrect = oDto.IsCorrect,
                    OrderIndex = oDto.OrderIndex
                };
                
                _context.Options.Add(option);
            }
        }


        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
