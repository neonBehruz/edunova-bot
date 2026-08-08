using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class LevelKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🟢 A1", "🟢 A2" },
            new KeyboardButton[] { "🟡 B1", "🟡 B2" },
            new KeyboardButton[] { "🔵 C1", "🔵 C2" },
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
