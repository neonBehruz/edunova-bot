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

        string emoji = result.ScorePercentage >= 80 ? "🏆" : result.ScorePercentage >= 60 ? "👏" : "💪";

        string messageText = $"{emoji} *TEST YAKUNLANDI*\n\n" +
                             $"📌 CEFR Daraja: *{result.Level}*\n" +
                             $"⚡ Qiyinchilik: *{result.Difficulty}*\n" +
                             $"❓ Jami savollar: *{result.TotalQuestions} ta*\n" +
                             $"✅ To'g'ri javoblar: *{result.CorrectAnswers} ta*\n" +
                             $"❌ Noto'g'ri javoblar: *{result.IncorrectAnswers} ta*\n" +
                             $"📊 Foiz: *{result.ScorePercentage}%*\n" +
                             $"⏱️ Ketgan vaqt: *{result.DurationSeconds} s*\n" +
                             $"⭐ Qo'shilgan XP: *+{result.RatingPointsEarned} XP*\n\n" +
                             $"📝 *SAVOLLAR TAHLILI:*\n";

        int index = 1;
        foreach (var review in result.QuestionReviews)
        {
            string icon = review.IsCorrect ? "✅" : "❌";
            string safeQ = review.QuestionText.Replace("_", "\\_");
            string safeAns = (review.YourAnswer ?? "").Replace("_", "\\_");
            string safeCorr = (review.CorrectAnswer ?? "").Replace("_", "\\_");

            messageText += $"{index}. {icon} {safeQ}\n" +
                           $"   Sizning javobingiz: {safeAns}\n" +
                           $"   To'g'ri javob: *{safeCorr}*\n";
            if (!string.IsNullOrEmpty(review.Explanation))
            {
                string safeExp = review.Explanation.Replace("_", "\\_");
                messageText += $"   💡 *Izoh:* {safeExp}\n";
            }
            messageText += "\n";
            index++;
        }

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
