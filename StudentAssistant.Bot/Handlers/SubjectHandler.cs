using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class SubjectHandler
{
    private readonly TestSessionManager _sessionManager;

    public SubjectHandler(TestSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task HandlePromptAsync(ITelegramBotClient botClient, Message message, long telegramId, CancellationToken cancellationToken)
    {
        _sessionManager.SetUserState(telegramId, UserStateStep.SelectingSubject);

        var selections = _sessionManager.GetUserSelections(telegramId);
        int grade = selections.Grade > 0 ? selections.Grade : 1;

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: $"📚 *{grade}-sinf uchun test topshirmoqchi bo'lgan fanningizni tanlang:*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: SubjectKeyboard.GetKeyboardForGrade(grade),
            cancellationToken: cancellationToken);
    }

    public bool TryParseSubject(string input, out long subjectId)
    {
        string text = input.Trim();

        if (text.Contains("Ingliz tili")) { subjectId = 1; return true; }
        if (text.Contains("Tarix") || text.Contains("O'zbekiston Tarixi") || text.Contains("Jahon Tarixi")) { subjectId = 2; return true; }
        if (text.Contains("Matematika") || text.Contains("Algebra") || text.Contains("Geometriya")) { subjectId = 3; return true; }
        if (text.Contains("Ona tili") || text.Contains("O'zbek tili") || text.Contains("Adabiyot")) { subjectId = 4; return true; }
        if (text.Contains("Fizika")) { subjectId = 5; return true; }
        if (text.Contains("Kimyo")) { subjectId = 6; return true; }
        if (text.Contains("Biologiya") || text.Contains("Botanika") || text.Contains("Tabiiy fan")) { subjectId = 7; return true; }
        if (text.Contains("Informatika")) { subjectId = 8; return true; }
        if (text.Contains("Rus tili")) { subjectId = 9; return true; }
        if (text.Contains("Geografiya")) { subjectId = 10; return true; }
        if (text.Contains("Tasviriy san'at")) { subjectId = 11; return true; }
        if (text.Contains("Musiqa")) { subjectId = 12; return true; }
        if (text.Contains("Texnologiya")) { subjectId = 13; return true; }
        if (text.Contains("Tarbiya")) { subjectId = 14; return true; }
        if (text.Contains("Huquq")) { subjectId = 15; return true; }

        subjectId = 1;
        return false;
    }
}
