namespace ScholarFlow.Application.DTOs;

public class AtomicSubTopicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class AtomicTopicDto
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public List<AtomicSubTopicDto> SubTopics { get; set; } = new();
}

public class AtomicSubjectDto
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<AtomicTopicDto> Topics { get; set; } = new();
}

public class AcademicStreamDto
{
    public Guid StreamId { get; set; }
    public string StreamName { get; set; } = string.Empty;
    public List<AtomicSubjectDto> Subjects { get; set; } = new();
}
