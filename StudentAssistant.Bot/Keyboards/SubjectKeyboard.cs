using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class SubjectKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "📜 Tarix", "📐 Matematika" },
            new KeyboardButton[] { "📚 O'zbek tili", "🇬🇧 Ingliz tili" },
            new KeyboardButton[] { "⚡ Fizika", "🧪 Kimyo" },
            new KeyboardButton[] { "🌿 Biologiya", "💻 Informatika" },
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
