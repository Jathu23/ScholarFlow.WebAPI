using MediatR;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.Application.Features.Subjects.Commands.BulkCreateAcademicStructure;

public class BulkCreateAcademicStructureCommandHandler : IRequestHandler<BulkCreateAcademicStructureCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public BulkCreateAcademicStructureCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(BulkCreateAcademicStructureCommand request, CancellationToken cancellationToken)
    {
        // 1. Gather all unique names from the request to pre-fetch data
        var streamNames = request.Streams.Select(s => s.StreamName).Distinct().ToList();
        
        var allSubjects = request.Streams.SelectMany(s => s.Subjects).ToList();
        var subjectNames = allSubjects.Select(s => s.SubjectName).Distinct().ToList();
        
        var allTopics = allSubjects.SelectMany(s => s.Topics).ToList();
        var topicNames = allTopics.Select(t => t.TopicName).Distinct().ToList();
        
        var allSubTopics = allTopics.SelectMany(t => t.SubTopics).ToList();
        var subTopicNames = allSubTopics.Select(st => st.Name).Distinct().ToList();

        // 2. Fetch existing entities into memory
        // We fetch everything that *might* be relevant to avoid N+1 lookups
        var existingStreams = await _context.Streams
            .Include(s => s.SubjectStreams)
            .Where(s => streamNames.Contains(s.Name))
            .ToListAsync(cancellationToken);

        var existingSubjects = await _context.Subjects
            .Include(s => s.SubjectStreams)
            .Where(s => subjectNames.Contains(s.Name))
            .ToListAsync(cancellationToken);

        var existingTopics = await _context.Topics
            .Where(t => topicNames.Contains(t.TopicName))
            .ToListAsync(cancellationToken);
            
        var existingSubTopics = await _context.SubTopics
            .Where(st => subTopicNames.Contains(st.SubTopicName))
            .ToListAsync(cancellationToken);

        // Helper list to track links (both existing and new)
        var knownLinks = existingStreams.SelectMany(s => s.SubjectStreams).ToList();

        // 3. Process the hierarchy using in-memory lookups
        foreach (var streamDto in request.Streams)
        {
            var stream = existingStreams.FirstOrDefault(s => s.Name == streamDto.StreamName);
            if (stream == null)
            {
                stream = new AcademicStream { Name = streamDto.StreamName };
                existingStreams.Add(stream);
                _context.Streams.Add(stream);
            }

            foreach (var subjectDto in streamDto.Subjects)
            {
                var subject = existingSubjects.FirstOrDefault(s => s.Name == subjectDto.SubjectName);
                if (subject == null)
                {
                    subject = new Subject { Name = subjectDto.SubjectName };
                    existingSubjects.Add(subject);
                    _context.Subjects.Add(subject);
                }

                // Ensure Stream-Subject Link
                // Check if link exists in our known list
                // We can rely on object reference equality for new/tracked entities
                var linkExists = knownLinks.Any(ss => 
                    (ss.Stream == stream && ss.Subject == subject) ||
                    (ss.StreamId != Guid.Empty && ss.StreamId == stream.Id && ss.SubjectId != Guid.Empty && ss.SubjectId == subject.Id)
                );

                if (!linkExists)
                {
                    var link = new SubjectStream { Stream = stream, Subject = subject };
                    knownLinks.Add(link);
                    _context.SubjectStreams.Add(link); 
                }

                foreach (var topicDto in subjectDto.Topics)
                {
                    // Find topic that belongs to this specific subject
                    var topic = existingTopics.FirstOrDefault(t => 
                        t.TopicName == topicDto.TopicName && 
                        ((t.SubjectId != Guid.Empty && t.SubjectId == subject.Id) || t.Subject == subject));

                    if (topic == null)
                    {
                        topic = new Topic 
                        { 
                            TopicName = topicDto.TopicName, 
                            Subject = subject 
                        };
                        existingTopics.Add(topic);
                        _context.Topics.Add(topic);
                    }

                    foreach (var subTopicDto in topicDto.SubTopics)
                    {
                        var subTopic = existingSubTopics.FirstOrDefault(st => 
                            st.SubTopicName == subTopicDto.Name && 
                            ((st.TopicId != Guid.Empty && st.TopicId == topic.Id) || st.Topic == topic));

                        if (subTopic == null)
                        {
                            subTopic = new SubTopic 
                            { 
                                SubTopicName = subTopicDto.Name, 
                                Topic = topic 
                            };
                            existingSubTopics.Add(subTopic);
                            _context.SubTopics.Add(subTopic);
                        }
                    }
                }
            }
        }

        // 4. Single SaveChanges to persist the entire graph
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<bool>.Success(true);
    }
}
