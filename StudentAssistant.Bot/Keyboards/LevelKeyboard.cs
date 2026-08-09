using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class LevelKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🏫 5-sinf", "🏫 6-sinf" },
            new KeyboardButton[] { "🏫 7-sinf", "🏫 8-sinf" },
            new KeyboardButton[] { "🏫 9-sinf", "🏫 10-sinf" },
            new KeyboardButton[] { "🏫 11-sinf", "🎓 C2 Oliy Daraja" },
            new KeyboardButton[] { "🟢 A1", "🟢 A2", "🟡 B1" },
            new KeyboardButton[] { "🟡 B2", "🔵 C1", "🔵 C2" },
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
