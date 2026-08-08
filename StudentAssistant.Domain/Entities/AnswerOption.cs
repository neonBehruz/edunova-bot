using StudentAssistant.Domain.Common;

namespace StudentAssistant.Domain.Entities;

public class AnswerOption : Auditable
{
    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}
