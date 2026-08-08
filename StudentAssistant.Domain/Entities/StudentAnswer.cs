using StudentAssistant.Domain.Common;

namespace StudentAssistant.Domain.Entities;

public class StudentAnswer : Auditable
{
    public long TestAttemptId { get; set; }
    public TestAttempt TestAttempt { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public long? SelectedOptionId { get; set; }
    public AnswerOption? SelectedOption { get; set; }

    public bool IsCorrect { get; set; }
    public int TimeTakenSeconds { get; set; }
}
