using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class LevelKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🏫 1-sinf", "🏫 2-sinf", "🏫 3-sinf", "🏫 4-sinf" },
            new KeyboardButton[] { "🏫 5-sinf", "🏫 6-sinf", "🏫 7-sinf", "🏫 8-sinf" },
            new KeyboardButton[] { "🏫 9-sinf", "🏫 10-sinf", "🏫 11-sinf" },
            new KeyboardButton[] { "🟢 A1", "🟢 A2", "🟡 B1", "🟡 B2" },
            new KeyboardButton[] { "🔵 C1", "🔵 C2", "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
