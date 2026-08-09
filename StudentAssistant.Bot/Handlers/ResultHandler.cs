using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class ResultHandler
{
    private readonly ITestService _testService;
    private readonly ITestAttemptService _attemptService;
    private readonly IProgressService _progressService;
    private readonly TestSessionManager _sessionManager;

    public ResultHandler(
        ITestService testService,
        ITestAttemptService attemptService,
        IProgressService progressService,
        TestSessionManager sessionManager)
    {
        _testService = testService;
        _attemptService = attemptService;
        _progressService = progressService;
        _sessionManager = sessionManager;
    }

    public async Task FinalizeAndSendResultAsync(ITelegramBotClient botClient, TestSession session, CancellationToken cancellationToken)
    {
        var result = await _testService.SubmitTestAsync(session.AttemptId, session.UserAnswers);

        _sessionManager.RemoveSession(session.UserId);
        _sessionManager.SetUserState(session.UserId, UserStateStep.MainMenu);

        string messageText = "🏆 *TEST YAKUNLANDI*\n\n" +
                             $"📖 *{session.SubjectName}*\n\n" +
                             "╭──────────────╮\n" +
                             $"│ 📝 Savollar   {result.TotalQuestions}\n" +
                             $"│ ✅ To‘g‘ri     {result.CorrectAnswers}\n" +
                             $"│ ❌ Noto‘g‘ri   {result.IncorrectAnswers}\n" +
                             $"│ 📊 Natija      {result.ScorePercentage}%\n" +
                             $"│ ⏱️ Vaqt        {result.DurationSeconds} s\n" +
                             $"│ ⭐ XP          +{result.RatingPointsEarned}\n" +
                             "╰──────────────╯\n\n" +
                             "🚀 *Keyingi testda omad!*";

        await botClient.SendMessage(
            chatId: session.TelegramChatId,
            text: messageText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: ResultKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public async Task SendUserHistoryAsync(ITelegramBotClient botClient, Message message, long userId, CancellationToken cancellationToken)
    {
        var progress = await _progressService.GetUserProgressAsync(userId);
        var history = await _attemptService.GetUserHistoryAsync(userId, 100);

        int totalCorrect = history.Sum(h => h.CorrectAnswers);
        int totalQuestions = history.Sum(h => h.TotalQuestions);
        int totalIncorrect = totalQuestions - totalCorrect;
        double avgScore = history.Any() ? Math.Round(history.Average(h => h.ScorePercentage), 1) : 0;

        string text = "📊 *Natijalarim*\n\n" +
                      $"✅ *To'g'ri javoblar:* {totalCorrect}\n" +
                      $"❌ *Noto'g'ri javoblar:* {totalIncorrect}\n" +
                      $"📈 *O'rtacha natija:* {avgScore}%\n\n";

        foreach (var lp in progress.LevelProgresses)
        {
            int filled = (int)Math.Round(lp.ProgressPercentage / 10);
            if (filled > 10) filled = 10;
            string bar = new string('█', filled) + new string('░', 10 - filled);
            text += $"`{lp.Level,-2}` `{bar}` *{lp.ProgressPercentage}%*\n";
        }

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: MainMenuKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }
}
