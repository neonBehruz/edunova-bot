using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class DifficultyHandler
{
    private readonly TestSessionManager _sessionManager;

    public DifficultyHandler(TestSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task HandlePromptAsync(ITelegramBotClient botClient, Message message, long userId, CefrLevel level, CancellationToken cancellationToken)
    {
        _sessionManager.SetUserState(userId, UserStateStep.SelectingDifficulty);
        _sessionManager.SetUserLevelSelection(userId, level);

        string text = "🔥 *Qiyinchilik darajasini tanlang:*";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: DifficultyKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public bool TryParseDifficulty(string text, out DifficultyLevel difficulty)
    {
        string cleaned = text.Replace("🟢", "").Replace("🟡", "").Replace("🔴", "").Trim().ToLowerInvariant();

        if (cleaned.Contains("oson") || cleaned.Contains("easy")) { difficulty = DifficultyLevel.Easy; return true; }
        if (cleaned.Contains("o'rta") || cleaned.Contains("orta") || cleaned.Contains("middle")) { difficulty = DifficultyLevel.Middle; return true; }
        if (cleaned.Contains("qiyin") || cleaned.Contains("hard")) { difficulty = DifficultyLevel.Hard; return true; }

        return Enum.TryParse(cleaned, true, out difficulty);
    }
}
