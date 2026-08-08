using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Questions;

public class QuestionDto
{
    public long Id { get; set; }
    public long SubjectId { get; set; }
    public string Text { get; set; } = string.Empty;
    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public QuestionType Type { get; set; }
    public string? Explanation { get; set; }
    public List<AnswerOptionDto> Options { get; set; } = new();
}

public class AnswerOptionDto
{
    public long Id { get; set; }
    public long QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}
