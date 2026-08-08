using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Results;

public class TestResultDto
{
    public long AttemptId { get; set; }
    public long UserId { get; set; }
    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public double ScorePercentage { get; set; }
    public int DurationSeconds { get; set; }
    public int RatingPointsEarned { get; set; }
    public TestStatus Status { get; set; }
    public DateTime CompletedAt { get; set; }
    public List<QuestionReviewDto> QuestionReviews { get; set; } = new();
}

public class QuestionReviewDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? YourAnswer { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
}

public class ResultHistoryDto
{
    public long AttemptId { get; set; }
    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double ScorePercentage { get; set; }
    public DateTime CompletedAt { get; set; }
}
