using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using Telegram.Bot;

namespace StudentAssistant.Bot.Handlers;

public class QuestionHandler
{
    public async Task SendCurrentQuestionAsync(ITelegramBotClient botClient, TestSession session, CancellationToken cancellationToken)
    {
        if (session.CurrentQuestionIndex >= session.Questions.Count) return;

        var question = session.Questions[session.CurrentQuestionIndex];
        session.QuestionStartedAt = DateTime.UtcNow;

        string header = $"*{session.SelectedLevel} • {session.SelectedDifficulty}*\t\t\t\t\t\t\t\t\t\t*Savol {session.CurrentQuestionIndex + 1} / {session.Questions.Count}*\n\n";

        string safeText = question.Text.Replace("_", "\\_");
        string questionText = $"{header}Choose the correct answer:\n{safeText}";

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
