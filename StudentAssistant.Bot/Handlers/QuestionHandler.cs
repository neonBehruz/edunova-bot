using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using Telegram.Bot;

namespace StudentAssistant.Bot.Handlers;

public class QuestionHandler
{
    public async Task SendCurrentQuestionAsync(ITelegramBotClient botClient, TestSession session, CancellationToken cancellationToken)
    {
        if (session.CurrentQuestionIndex >= session.Questions.Count) return;

        var question = session.Questions[session.CurrentQuestionIndex];
        session.QuestionStartedAt = DateTime.UtcNow;

        string diffUz = session.SelectedDifficulty switch
        {
            DifficultyLevel.Easy => "Oson",
            DifficultyLevel.Middle => "O'rta",
            DifficultyLevel.Hard => "Qiyin",
            _ => session.SelectedDifficulty.ToString()
        };

        string header = $"📌 *{session.SelectedLevel} • {diffUz}*\t\t\t*Savol {session.CurrentQuestionIndex + 1} / {session.Questions.Count}*\n\n";

        string safeText = question.Text.Replace("_", "\\_");
        string questionText = $"{header}❓ *To'g'ri javobni tanlang:*\n{safeText}";

        var keyboard = AnswerKeyboard.GetKeyboard(session.AttemptId, question.Id, question.Options.ToList());

        var msg = await botClient.SendMessage(
            chatId: session.TelegramChatId,
            text: questionText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);

        session.CurrentMessageId = msg.MessageId;
    }
}
