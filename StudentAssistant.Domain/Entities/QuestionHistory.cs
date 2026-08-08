using StudentAssistant.Domain.Common;

namespace StudentAssistant.Domain.Entities;

public class QuestionHistory : Auditable
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public DateTime LastAnsweredAt { get; set; } = DateTime.UtcNow;
    public int TimesAnswered { get; set; } = 0;
    public int TimesCorrect { get; set; } = 0;
}
