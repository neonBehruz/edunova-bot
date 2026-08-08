using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Tests;

public class StartTestDto
{
    public long UserId { get; set; }
    public long SubjectId { get; set; } = 1;
    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int QuestionCount { get; set; } = 5;
}

public class TestQuestionDto
{
    public long QuestionId { get; set; }
    public int QuestionIndex { get; set; }
    public int TotalQuestions { get; set; }
    public string Text { get; set; } = string.Empty;
    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public List<TestOptionDto> Options { get; set; } = new();
}

public class TestOptionDto
{
    public long OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class SubmitAnswerDto
{
    public long AttemptId { get; set; }
    public long QuestionId { get; set; }
    public long? SelectedOptionId { get; set; }
    public int TimeTakenSeconds { get; set; }
}
