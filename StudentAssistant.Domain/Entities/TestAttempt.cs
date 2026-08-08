using StudentAssistant.Domain.Common;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Domain.Entities;

public class TestAttempt : Auditable
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public CefrLevel Level { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double ScorePercentage { get; set; }
    public TestStatus Status { get; set; } = TestStatus.InProcess;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int DurationSeconds { get; set; }

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
