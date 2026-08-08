using StudentAssistant.Domain.Common;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Domain.Entities;

public class Question : Auditable
{
    public long SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public CefrLevel Level { get; set; } = CefrLevel.A1;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
    public QuestionType Type { get; set; } = QuestionType.SingleChoice;
    public string? Explanation { get; set; }

    public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
}
