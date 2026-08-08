using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class DifficultyKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🟢 Easy" },
            new KeyboardButton[] { "🟡 Middle" },
            new KeyboardButton[] { "🔴 Hard" },
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
