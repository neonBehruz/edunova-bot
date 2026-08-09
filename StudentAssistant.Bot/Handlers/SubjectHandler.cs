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

    public bool TryParseSubject(string input, out long subjectId, out string subjectName)
    {
        string text = input.Trim();

        if (text.Contains("Ingliz tili")) { subjectId = 1; subjectName = "Ingliz tili"; return true; }
        if (text.Contains("O'zbekiston Tarixi")) { subjectId = 2; subjectName = "O'zbekiston Tarixi"; return true; }
        if (text.Contains("Jahon Tarixi")) { subjectId = 2; subjectName = "Jahon Tarixi"; return true; }
        if (text.Contains("Tarix")) { subjectId = 2; subjectName = "Tarix"; return true; }
        if (text.Contains("Algebra")) { subjectId = 3; subjectName = "Algebra"; return true; }
        if (text.Contains("Geometriya")) { subjectId = 3; subjectName = "Geometriya"; return true; }
        if (text.Contains("Matematika")) { subjectId = 3; subjectName = "Matematika"; return true; }
        if (text.Contains("Ona tili va Adabiyot")) { subjectId = 4; subjectName = "Ona tili va Adabiyot"; return true; }
        if (text.Contains("Ona tili")) { subjectId = 4; subjectName = "Ona tili"; return true; }
        if (text.Contains("O'zbek tili")) { subjectId = 4; subjectName = "O'zbek tili"; return true; }
        if (text.Contains("Adabiyot")) { subjectId = 4; subjectName = "Adabiyot"; return true; }
        if (text.Contains("Fizika")) { subjectId = 5; subjectName = "Fizika"; return true; }
        if (text.Contains("Kimyo")) { subjectId = 6; subjectName = "Kimyo"; return true; }
        if (text.Contains("Botanika")) { subjectId = 7; subjectName = "Botanika"; return true; }
        if (text.Contains("Tabiiy fan")) { subjectId = 7; subjectName = "Tabiiy fan"; return true; }
        if (text.Contains("Biologiya")) { subjectId = 7; subjectName = "Biologiya"; return true; }
        if (text.Contains("Informatika")) { subjectId = 8; subjectName = "Informatika"; return true; }
        if (text.Contains("Rus tili")) { subjectId = 9; subjectName = "Rus tili"; return true; }
        if (text.Contains("Geografiya")) { subjectId = 10; subjectName = "Geografiya"; return true; }
        if (text.Contains("Tasviriy san'at")) { subjectId = 11; subjectName = "Tasviriy san'at"; return true; }
        if (text.Contains("Musiqa")) { subjectId = 12; subjectName = "Musiqa"; return true; }
        if (text.Contains("Texnologiya")) { subjectId = 13; subjectName = "Texnologiya"; return true; }
        if (text.Contains("Tarbiya")) { subjectId = 14; subjectName = "Tarbiya"; return true; }
        if (text.Contains("Huquq")) { subjectId = 15; subjectName = "Huquq"; return true; }

        subjectId = 1;
        subjectName = "Ingliz tili";
        return false;
    }
}
