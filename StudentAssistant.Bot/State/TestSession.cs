using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Tests;

namespace StudentAssistant.Bot.State;

public class TestSession
{
    public long UserId { get; set; }
    public long TelegramChatId { get; set; }
    public long AttemptId { get; set; }
    public CefrLevel SelectedLevel { get; set; }
    public long SubjectId { get; set; } = 1;
    public string SubjectName { get; set; } = "Ingliz tili";
    public DifficultyLevel SelectedDifficulty { get; set; }
    public int RequestedQuestionCount { get; set; }

    public List<Question> Questions { get; set; } = new();
    public int CurrentQuestionIndex { get; set; } = 0;
    public List<SubmitAnswerDto> UserAnswers { get; set; } = new();

    public int? CurrentMessageId { get; set; }
    public DateTime QuestionStartedAt { get; set; }
    public DateTime SessionExpiresAt { get; set; }
    public bool IsTimedOut { get; set; } = false;
}
