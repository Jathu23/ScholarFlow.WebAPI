namespace ScholarFlow.Application.DTOs;

public class CreateSubTopicDto
{
    public string Name { get; set; } = string.Empty;
}

public class CreateTopicDto
{
    public string TopicName { get; set; } = string.Empty;
    public List<CreateSubTopicDto> SubTopics { get; set; } = new();
}

public class CreateSubjectDto
{
    public string SubjectName { get; set; } = string.Empty;
    public List<CreateTopicDto> Topics { get; set; } = new();
}

public class CreateStreamDto
{
    public string StreamName { get; set; } = string.Empty;
    public List<CreateSubjectDto> Subjects { get; set; } = new();
}
