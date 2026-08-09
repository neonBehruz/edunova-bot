using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class SubjectHandler
{
    private readonly TestSessionManager _sessionManager;
    private readonly DifficultyHandler _difficultyHandler;

    public SubjectHandler(TestSessionManager sessionManager, DifficultyHandler difficultyHandler)
    {
        _sessionManager = sessionManager;
        _difficultyHandler = difficultyHandler;
    }

    public async Task HandlePromptAsync(ITelegramBotClient botClient, Message message, long telegramId, CancellationToken cancellationToken)
    {
        _sessionManager.SetUserState(telegramId, UserStateStep.SelectingSubject);

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "📚 *Test topshirmoqchi bo'lgan fanningizni tanlang:*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: SubjectKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public bool TryParseSubject(string input, out long subjectId)
    {
        string text = input.Trim();
        if (text.Contains("Tarix")) { subjectId = 2; return true; }
        if (text.Contains("Matematika")) { subjectId = 3; return true; }
        if (text.Contains("O'zbek tili")) { subjectId = 4; return true; }
        if (text.Contains("Ingliz tili")) { subjectId = 1; return true; }
        if (text.Contains("Fizika")) { subjectId = 5; return true; }
        if (text.Contains("Kimyo")) { subjectId = 6; return true; }
        if (text.Contains("Biologiya")) { subjectId = 7; return true; }
        if (text.Contains("Informatika")) { subjectId = 8; return true; }

        subjectId = 1;
        return false;
    }
}
