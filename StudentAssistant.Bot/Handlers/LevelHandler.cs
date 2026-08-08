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

        string text = "🎯 *Darajangizni tanlang:*";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: LevelKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public bool TryParseLevel(string text, out CefrLevel level)
    {
        string cleaned = text.Replace("🟢", "").Replace("🟡", "").Replace("🔵", "").Trim();
        return Enum.TryParse(cleaned, true, out level);
    }
}
