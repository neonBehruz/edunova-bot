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

        string text = "🏫 *O'zingiz o'qiydigan Sinf / Darajangizni tanlang:*";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: LevelKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public bool TryParseLevel(string text, out CefrLevel level)
    {
        string cleaned = text.Replace("🏫", "").Replace("🎓", "").Replace("🟢", "").Replace("🟡", "").Replace("🔵", "").Trim();

        if (cleaned.Contains("5-sinf") || cleaned.Contains("6-sinf")) { level = CefrLevel.A1; return true; }
        if (cleaned.Contains("7-sinf") || cleaned.Contains("8-sinf")) { level = CefrLevel.A2; return true; }
        if (cleaned.Contains("9-sinf")) { level = CefrLevel.B1; return true; }
        if (cleaned.Contains("10-sinf")) { level = CefrLevel.B2; return true; }
        if (cleaned.Contains("11-sinf")) { level = CefrLevel.C1; return true; }

        return Enum.TryParse(cleaned, true, out level);
    }
}
