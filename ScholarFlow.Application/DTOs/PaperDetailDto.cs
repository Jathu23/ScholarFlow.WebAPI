namespace ScholarFlow.Application.DTOs;

public class PaperDetailDto : PaperDto
{
    public List<QuestionDto> Questions { get; set; } = new();
}
