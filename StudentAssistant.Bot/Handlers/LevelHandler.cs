using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class LevelHandler
{
    private readonly TestSessionManager _sessionManager;

    public LevelHandler(TestSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task HandlePromptAsync(ITelegramBotClient botClient, Message message, long userId, CancellationToken cancellationToken)
    {
        _sessionManager.SetUserState(userId, UserStateStep.SelectingLevel);

        string text = "🏫 *O'zingiz o'qiydigan Sinfni (1-11 sinf) tanlang:*";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: LevelKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public bool TryParseLevel(string text, out CefrLevel level, out int grade)
    {
        string cleaned = text.Replace("🏫", "").Trim();

        grade = 1;
        if (cleaned.Contains("1-sinf")) { grade = 1; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("2-sinf")) { grade = 2; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("3-sinf")) { grade = 3; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("4-sinf")) { grade = 4; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("5-sinf")) { grade = 5; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("6-sinf")) { grade = 6; level = CefrLevel.A1; return true; }
        if (cleaned.Contains("7-sinf")) { grade = 7; level = CefrLevel.A2; return true; }
        if (cleaned.Contains("8-sinf")) { grade = 8; level = CefrLevel.A2; return true; }
        if (cleaned.Contains("9-sinf")) { grade = 9; level = CefrLevel.B1; return true; }
        if (cleaned.Contains("10-sinf")) { grade = 10; level = CefrLevel.B2; return true; }
        if (cleaned.Contains("11-sinf")) { grade = 11; level = CefrLevel.C1; return true; }

        level = CefrLevel.A1;
        return false;
    }
}
